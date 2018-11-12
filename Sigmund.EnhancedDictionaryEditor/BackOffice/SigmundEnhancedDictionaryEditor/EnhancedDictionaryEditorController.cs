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
using Sigmund.EnhancedDictionaryEditor.Model.MenuAction;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using EnhancedDictionaryEditor.Model.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Xml;
using System.Net.Http;
using Sigmund.EnhancedDictionaryEditor.Model.Serialization;
using System.Web.Http;
using System.Net;
using System.Threading.Tasks;

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
        private const string RootNodeId = "-1";

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

        [HttpPost]
        public async Task<HttpResponseMessage> ImportXml()
        {
            SerializableDictionaryItemsArray deserializedXml;
            string xml = await Request.Content.ReadAsStringAsync();

            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionaryItemsArray));
            using (var stream = new StringReader(xml))
            {
                deserializedXml = serializer.Deserialize(stream) as SerializableDictionaryItemsArray;
            }
            var allItems = deserializedXml.DictionaryItems.Select(x => new ItemInfos(x)).ToList();
            var rootItems = allItems.Where(x => x.ParentId == null || !x.ParentId.HasValue).ToList();
            
            foreach (var dictionaryItem in rootItems)
            {
                ImportItemWithChildren(dictionaryItem, allItems);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        
        [HttpGet]
        public HttpResponseMessage ExportXml()
        {
            var dictionaryKeys = new SerializableDictionaryItemsArray() {
                DictionaryItems = GetAll().Select(x => new SerializableDictionaryItem(x)).ToArray()
            };

            XmlSerializer serializer = new XmlSerializer(dictionaryKeys.GetType());
            var xmlSettings = new XmlWriterSettings()
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                NewLineOnAttributes = false,
            };

            using (var ms = new MemoryStream())
            using (var xw = XmlWriter.Create(ms, xmlSettings))
            {
                serializer.Serialize(xw, dictionaryKeys);
                var encodedXml = Encoding.UTF8.GetString(ms.ToArray());

                return new HttpResponseMessage() { Content = new StringContent(encodedXml, Encoding.UTF8, "application/xml") };
            }
        }

        public void Save(ItemInfos dictionaryItem)
        {
            var item = dictionaryItem.IsNew ? null : LocalizationService.GetDictionaryItemById(dictionaryItem.Id.Value);
            var isNew = false;

            if (item == null)
            {
                isNew = true;
                item = new DictionaryItem(dictionaryItem.ParentId, dictionaryItem.Key);

                //to be assured that the new item will have the same id as the imported one.
                if (dictionaryItem.Id != null && dictionaryItem.Id.HasValue)
                {
                    item.Key = dictionaryItem.Id.Value;
                }
            } else
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
            if (id == RootNodeId)
            {
                var importMenuItem = new ImportDictionaryItems
                {
                    SeperatorBefore = true
                };

                menuItems.Items.Add(importMenuItem);
                menuItems.Items.Add(new ExportDictionaryItems());
            }
            else
            {
                menuItems.Items.Add<ActionDelete>("Delete");
            }

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

        private void ImportItemWithChildren(ItemInfos importedDictionaryItem, List<ItemInfos> itemsToImport)
        {
            var childrens = itemsToImport.Where(x => x.ParentId == importedDictionaryItem.Id).ToList();
            Save(importedDictionaryItem);

            foreach (var child in childrens)
            {
                child.ParentId = importedDictionaryItem.Id;
            }

            foreach (var child in childrens)
            {
                ImportItemWithChildren(child, itemsToImport);
            }
        }
    }
}
