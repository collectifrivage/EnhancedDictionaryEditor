using System;

namespace Sigmund.EnhancedDictionaryEditor.Translation
{
    public static class DictionaryValueTranslatorProvider
    {
        private static ITranslator _translator;

        public static ITranslator GetTranslator()
        {
            return _translator;
        }

        public static void SetTranslator(ITranslator translator)
        {
            _translator = translator ?? throw new ArgumentNullException("translator");
        }
    }
}