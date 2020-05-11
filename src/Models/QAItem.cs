namespace QAHelper.Models
{
    public class QAItem
    {
        public string Question { get; set; } = string.Empty;

        public string[] Answers { get; set; } = new string[0];
    }
}