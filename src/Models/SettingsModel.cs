using NotesSearcher.Enums;

namespace NotesSearcher.Models;
public class SettingsModel
{
    public RecognitionLanguage ImageRecognitionLanguage { get; set; } = RecognitionLanguage.EN;

    public char[] Punctuation { get; set; } = new char[] { '.', ',', '!', '?', ':', ';', '-', '(', ')' };

    public KeyWordsSearchType KeyWordsSearchType { get; set; } = KeyWordsSearchType.Both;
}
