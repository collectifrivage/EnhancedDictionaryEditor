using Sigmund.EnhancedDictionaryEditor.Model.Serialization;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace EnhancedDictionaryEditor.Model.Serialization
{
    [Serializable]
    public class SerializableDictionaryItem
    {
        public Guid? Id { get; set; }

        public string Key { get; set; }

        [XmlArray("Translations")]
        [XmlArrayItem("Translation")]
        public SerializableItemInfosTranslation[] Translations { get; set; }

        public Guid? ParentId { get; set; }

        public bool IsNew => Id == null;

        public SerializableDictionaryItem()
        {
            
        }

        public SerializableDictionaryItem(ItemInfos item)
        {
            Id = item.Id;
            Key = item.Key;
            Translations = item.Values.Select(x => new SerializableItemInfosTranslation(x.Key, x.Value)).ToArray();
            ParentId = item.ParentId;
        }
    }
}
