using QAHelper.Enums;

namespace QAHelper.Models
{
    public class SettingsModel
    {
        public RecognitionLanguage ImageRecognitionLanguage { get; set; } = RecognitionLanguage.EN;
    }
}