using EnhancedDictionaryEditor.Model;
using EnhancedDictionaryEditor.Model.Serialization;
using Sigmund.EnhancedDictionaryEditor.Model;
using Sigmund.EnhancedDictionaryEditor.Model.MenuAction;
using Sigmund.EnhancedDictionaryEditor.Model.Serialization;
using Sigmund.EnhancedDictionaryEditor.Repository;
using Sigmund.EnhancedDictionaryEditor.Translation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using umbraco.BusinessLogic.Actions;
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
        private ItemInfosRepository ItemInfosRepository;
        
        private const string RootNodeId = "-1";

        public EnhancedDictionaryEditorController()
        {
            TreeNodeRepository = new DictionnaryKeyTreeNodeRepository(this);
            ItemInfosRepository = new ItemInfosRepository();
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

            return ItemInfosRepository.GetDictionaryItemById(keyGuid);
        }

        public void DeleteById(string id)
        {
            ItemInfosRepository.DeleteById(id);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ImportXml()
        {
            SerializableDictionaryItemsArray deserializedXml;
            string xml = await Request.Content.ReadAsStringAsync();

            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionaryItemsArray));
            
            using (var stream = new StringReader(xml))
            {
                try
                {
                    deserializedXml = serializer.Deserialize(stream) as SerializableDictionaryItemsArray;
                }
                catch (InvalidOperationException e)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = e.Message
                    };
                }
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

        public ItemInfos Save(ItemInfos dictionaryItem)
        {
            ItemInfosRepository.Save(dictionaryItem);

            return dictionaryItem;
        }

        /// <summary>
        /// GET: /umbraco/backoffice/SigmundEnhancedDictionaryEditor/Translate/
        /// Return: Translated text.
        [HttpGet]
        public HttpResponseMessage Translate(string text, string translate_from, string translate_to)
        {
            if (!TranslationAvailable()) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var translator = DictionaryValueTranslatorProvider.GetTranslator();
            var translatedText = translator.Translate(text, translate_from, translate_to);

            return new HttpResponseMessage() {
                Content = new StringContent(translatedText)
            };
        }

        /// <summary>
        /// GET: /umbraco/backoffice/SigmundEnhancedDictionaryEditor/TranslationAvailable/
        /// Return: true if the auto translation feature is enabled.
        [HttpGet]
        public bool TranslationAvailable()
        {
            return DictionaryValueTranslatorProvider.GetTranslator() != null;
        }

        /// <summary>
        /// GET: /umbraco/backoffice/SigmundEnhancedDictionaryEditor/EnhancedDictionaryEditor/GetAll
        /// Return dictionary keys.
        /// </summary>
        /// <returns></returns>
        public ItemInfos[] GetAll()
        {
            return ItemInfosRepository.GetDictionaryItemDescendants()
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
            return UmbracoContext.Current.Application.Services.LocalizationService
                .GetAllLanguages()
                .Select(x => new ItemCulture(x))
                .ToArray();
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

        private void ImportItemWithChildren(ItemInfos importedDictionaryItem, List<ItemInfos> itemsToImport)
        {
            var childrens = itemsToImport.Where(x => x.ParentId == importedDictionaryItem.Id).ToList();
            ItemInfosRepository.Save(importedDictionaryItem);

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
