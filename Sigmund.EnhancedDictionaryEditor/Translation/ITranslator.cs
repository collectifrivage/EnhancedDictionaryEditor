namespace Sigmund.EnhancedDictionaryEditor.Translation
{
    public interface ITranslator
    {
        string Translate(string text, string languageTranslateFrom, string languageTranslateTo);
    }
}
