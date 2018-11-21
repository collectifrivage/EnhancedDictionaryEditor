using EnhancedDictionaryEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Sigmund.EnhancedDictionaryEditor.Repository
{
    public class ItemInfosRepository
    {
        private ILocalizationService LocalizationService;
        private IUserService UserService => UmbracoContext.Current.Application.Services.UserService;
        private int CurrentUserId => UserService.GetByUsername(HttpContext.Current.User.Identity.Name).Id;

        public ItemInfosRepository()
        {
            LocalizationService = UmbracoContext.Current.Application.Services.LocalizationService;
        }

        public List<ItemInfos> GetDictionaryItemDescendants(Guid? parentId = null)
        {
            return LocalizationService
                .GetDictionaryItemDescendants(parentId)
                .Select(x => new ItemInfos(x))
                .ToList();
        }

        public void DeleteById(string id)
        {
            Guid keyGuid;
            try
            {
                keyGuid = Guid.Parse(id);
            }
            catch (ArgumentNullException)
            {
                return;
            }
            var dictionaryItem = LocalizationService.GetDictionaryItemById(keyGuid);

            if (dictionaryItem != null)
            {
                LocalizationService.Delete(dictionaryItem, CurrentUserId);
            }
        }

        public void Save(ItemInfos dictionaryItem)
        {
            IDictionaryItem item = null;
            var isNew = dictionaryItem.IsNew;

            if (!dictionaryItem.IsNew)
            {
                item = LocalizationService.GetDictionaryItemByKey(dictionaryItem.Key);
                if(item == null)
                {
                    item = LocalizationService.GetDictionaryItemById(dictionaryItem.Id.Value);
                }
            } 

            if (item == null)
            {
                isNew = true;
                item = new DictionaryItem(dictionaryItem.ParentId, dictionaryItem.Key);

                //to be assured that the new item will have the same id as the imported one.
                if (dictionaryItem.Id != null && dictionaryItem.Id.HasValue)
                {
                    item.Key = dictionaryItem.Id.Value;
                }
            }
            else
            {
                item.ItemKey = dictionaryItem.Key;
                if (dictionaryItem.ParentId != null) item.ParentId = dictionaryItem.ParentId;
            }

            UpdateLanguages(item, dictionaryItem.Values);
            LocalizationService.Save(item, CurrentUserId);

            if (isNew)
            {
                //update saved item to have the newly created dictionary item Id.
                dictionaryItem.Id = item.Key;
            }
        }

        public ItemInfos GetDictionaryItemById(Guid id)
        {
            var item = LocalizationService.GetDictionaryItemById(id);

            return item == null ? null : new ItemInfos(item);
        }

        private void UpdateLanguages(IDictionaryItem item, IDictionary<string, string> values)
        {
            var languages = LocalizationService
                .GetAllLanguages()
                .ToArray();

            foreach (var value in values)
            {
                var language = languages.FirstOrDefault(x => x.CultureInfo.Name == value.Key);
                if (language == null) continue;

                LocalizationService.AddOrUpdateDictionaryValue(item, language, value.Value);
            }
        }
    }
}