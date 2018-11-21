using Umbraco.Web.Models.Trees;

namespace Sigmund.EnhancedDictionaryEditor.Model.MenuAction
{
    public class ExportDictionaryItems : MenuItem
    {
        private const string ExportTreeItemPath = "enhancedDictionaryEditor/EnhancedDictionaryEditorSectionTree/export/";

        public ExportDictionaryItems() : base("Export", "Export")
        {
            Icon = "arrow-down";
            NavigateToRoute(ExportTreeItemPath);
        }
    }
}