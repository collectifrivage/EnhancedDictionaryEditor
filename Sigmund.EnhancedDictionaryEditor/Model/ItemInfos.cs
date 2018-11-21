using EnhancedDictionaryEditor.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace EnhancedDictionaryEditor.Model
{
    public class ItemInfos
    {
        public Guid? Id { get; set; }
        public string Key { get; set; }
        public IDictionary<string, string> Values { get; set; }
        public Guid? ParentId { get; set; }

        public bool IsNew => Id == null;

        public ItemInfos()
        {
            
        }

        public ItemInfos(IDictionaryItem item)
        {
            Id = item.Key;
            Key = item.ItemKey;
            Values = item.Translations.ToDictionary(x => x.Language.CultureInfo.Name, x => x.Value);
            ParentId = item.ParentId;
        }

        public ItemInfos(SerializableDictionaryItem item)
        {
            Id = item.Id;
            Key = item.Key;
            Values = item.Translations.ToDictionary(x => x.Language, x => x.Text);
            ParentId = item.ParentId;
        }
    }
}
