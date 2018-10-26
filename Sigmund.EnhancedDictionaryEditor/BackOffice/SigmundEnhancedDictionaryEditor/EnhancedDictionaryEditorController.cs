using EnhancedDictionaryEditor.Model;
using Sigmund.EnhancedDictionaryEditor.BackOffice.SigmundEnhancedDictionaryEditor;
using Sigmund.EnhancedDictionaryEditor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace EnhancedDictionaryEditor
{
    [PluginController("SigmundEnhancedDictionaryEditor")]
    [Tree("enhancedDictionaryEditor", "EnhancedDictionaryEditorSectionTree", "Dictionnary", "icon-font")]
    public class EnhancedDictionaryEditorController : TreeController
    {
        private DictionnaryKeyTreeNodeRepository TreeNodeRepository;
        private ILocalizationService LocalizationService => UmbracoContext.Current.Application.Services.LocalizationService;
        private IUserService UserService => UmbracoContext.Current.Application.Services.UserService;
        private int CurrentUserId => UserService.GetByUsername(HttpContext.Current.User.Identity.Name).Id;

        public EnhancedDictionaryEditorController()
        {
            TreeNodeRepository = new DictionnaryKeyTreeNodeRepository(this);
        }
        
        public ItemInfos GetDictionaryItemById(string id)
        {
            Guid keyGuid;
            try
            {
                keyGuid = Guid.Parse(id);
            } catch(ArgumentNullException)
            {
                return null;
            }
            var dictionaryItem = LocalizationService.GetDictionaryItemById(keyGuid);
            
            return new ItemInfos(dictionaryItem);
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
            IDictionaryItem item;
            if (dictionaryItem.IsNew)
            {
                item = new DictionaryItem(dictionaryItem.ParentId, dictionaryItem.Key);
            } else
            {
                item = LocalizationService.GetDictionaryItemById(dictionaryItem.Id.Value);
                item.ItemKey = dictionaryItem.Key;
                if (dictionaryItem.ParentId != null) item.ParentId = dictionaryItem.ParentId;
            }
            
            UpdateLanguages(item, dictionaryItem.Values);
            LocalizationService.Save(item, CurrentUserId);
        }

        /// <summary>
        /// GET: /umbraco/backoffice/SigmundEnhancedDictionaryEditor/EnhancedDictionaryEditor/GetAll
        /// Return dictionary keys.
        /// </summary>
        /// <returns></returns>
        public ItemInfos[] GetAll()
        {
            return LocalizationService.GetDictionaryItemDescendants(null)
                .Select(x => new ItemInfos(x))
                .OrderBy(x => x.Key)
                .ToArray();
        }

        /// <summary>
        /// GET: /umbraco/backoffice/SigmundEnhancedDictionaryEditor/EnhancedDictionaryEditor/GetCultures
        /// Return the list of cultures
        /// </summary>
        /// <returns></returns>
        public ItemCulture[] GetCultures()
        {
            return LocalizationService.GetAllLanguages().Select(x => new ItemCulture(x)).ToArray();
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            return TreeNodeRepository.GetById(id);
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menuItems = new MenuItemCollection();
            menuItems.Items.Add(new CreateDictionaryItem(id));
            if (id != "-1") menuItems.Items.Add<ActionDelete>("Delete");

            return menuItems;
        }

        private void UpdateLanguages(IDictionaryItem item, IDictionary<string, string> values)
        {
            var languages = LocalizationService.GetAllLanguages().ToArray();

            foreach (var value in values)
            {
                var language = languages.FirstOrDefault(x => x.CultureInfo.Name == value.Key);
                if (language == null) continue;

                LocalizationService.AddOrUpdateDictionaryValue(item, language, value.Value);
            }
        }
    }
}
