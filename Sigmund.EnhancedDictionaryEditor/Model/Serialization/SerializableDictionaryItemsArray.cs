using EnhancedDictionaryEditor.Model.Serialization;
using System;
using System.Xml.Serialization;

namespace Sigmund.EnhancedDictionaryEditor.Model.Serialization
{
    [Serializable]
    [XmlRoot("DictionaryItemsExport")]
    public class SerializableDictionaryItemsArray
    {
        [XmlArray("DictionaryItems")]
        [XmlArrayItem("DictionaryItem")]
        public SerializableDictionaryItem[] DictionaryItems { get; set; }
    }
}