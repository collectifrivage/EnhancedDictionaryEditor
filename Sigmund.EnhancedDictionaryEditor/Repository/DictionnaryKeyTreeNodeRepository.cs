using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Sigmund.EnhancedDictionaryEditor.Repository
{
    public class DictionnaryKeyTreeNodeRepository
    {
        private ILocalizationService LocalizationService => UmbracoContext.Current.Application.Services.LocalizationService;
        private TreeController TreeController;
        private FormDataCollection EmptyFormData => new FormDataCollection(new List<KeyValuePair<string, string>>(0));
        private const string RootTreeNodeId = "-1";

        public DictionnaryKeyTreeNodeRepository(TreeController treeController)
        {
            TreeController = treeController;
        }

        public TreeNodeCollection GetById(string id)
        {
            var result = new TreeNodeCollection();
            IEnumerable<IDictionaryItem> keys;

            if (id == RootTreeNodeId)
            {
                keys = LocalizationService.GetRootDictionaryItems();
            } else
            {
                keys = LocalizationService.GetDictionaryItemChildren(Guid.Parse(id));
            }

            var nodes = keys
                .OrderBy(x => x.ItemKey)
                .Select(x => CreateTreeNode(x, id));

            result.AddRange(nodes);

            return result;
        }
        
        private TreeNode CreateTreeNode(IDictionaryItem item, string parentId)
        {
            var hasChildren = LocalizationService.GetDictionaryItemChildren(item.Key).Any();
            var node = TreeController.CreateTreeNode(item.Key.ToString(), parentId, EmptyFormData, item.ItemKey, "icon-book-alt", hasChildren);
            node.AdditionalData["key"] = item.ItemKey;

            return node;
        }
    }
}