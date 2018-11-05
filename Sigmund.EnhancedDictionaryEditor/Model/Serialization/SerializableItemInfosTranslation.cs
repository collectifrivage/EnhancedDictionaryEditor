using System;

namespace Sigmund.EnhancedDictionaryEditor.Model.Serialization
{
    [Serializable]
    public class SerializableItemInfosTranslation
    {
        public string Language;
        public string Text;

        public SerializableItemInfosTranslation()
        {
           
        }

        public SerializableItemInfosTranslation(string name, string text)
        {
            Language = name;
            Text = text;
        }
    }
}