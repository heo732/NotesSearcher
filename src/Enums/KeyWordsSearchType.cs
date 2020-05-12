using System.ComponentModel;

namespace QAHelper.Enums
{
    public enum KeyWordsSearchType
    {
        [Description("Questions")]
        Questions,
        [Description("Answers")]
        Answers,
        [Description("Questions and Answers")]
        Both
    }
}