using Umbraco.Web.Models.Trees;

namespace Sigmund.EnhancedDictionaryEditor.Model.MenuAction
{
    public class ImportDictionaryItems : MenuItem
    {
        private const string ImportTreeItemPath = "enhancedDictionaryEditor/EnhancedDictionaryEditorSectionTree/import/";

        public ImportDictionaryItems() : base("Import", "Import")
        {
            Icon = "arrow-up";
            NavigateToRoute(ImportTreeItemPath);
        }
    }
}