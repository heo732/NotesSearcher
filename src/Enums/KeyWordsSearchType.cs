using System.ComponentModel;

namespace NotesSearcher.Enums;
public enum KeyWordsSearchType
{
    [Description("Questions")]
    Questions,
    [Description("Answers")]
    Answers,
    [Description("Questions and Answers")]
    Both
}
