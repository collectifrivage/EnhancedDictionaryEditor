using System.Globalization;
using Umbraco.Core.Models;

namespace Sigmund.EnhancedDictionaryEditor.Model
{
    public class ItemCulture
    {
        public string Name { get; }
        public string DisplayName { get; }

        public ItemCulture(ILanguage culture)
        {
            Name = culture.CultureInfo.Name;
            DisplayName = culture.CultureInfo.DisplayName;
        }
    }
}