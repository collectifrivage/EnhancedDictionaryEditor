using Umbraco.Web.Models.Trees;

namespace Sigmund.EnhancedDictionaryEditor.Model
{
    public class CreateDictionaryItem : MenuItem
    {
        private const string EditTreeItemPath = "enhancedDictionaryEditor/EnhancedDictionaryEditorSectionTree/edit/";

        public CreateDictionaryItem(string parentId) : base("create", "Create")
        {
            Icon = "add";
            NavigateToRoute($"{EditTreeItemPath}?parent={parentId}");

        }
    }
}