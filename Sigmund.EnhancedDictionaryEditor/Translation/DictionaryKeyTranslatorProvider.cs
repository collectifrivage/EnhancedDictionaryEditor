using System;

namespace Sigmund.EnhancedDictionaryEditor.Translation
{
    public static class DictionaryKeyTranslatorProvider
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

    public class Test : ITranslator
    {
        public string Translate(string text, string languageTranslateFrom, string languageTranslateTo)
        {
            return "gfhfghgfhfgh";
        }
    }
}