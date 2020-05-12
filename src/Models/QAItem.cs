using System.Collections.Generic;

namespace QAHelper.Models
{
    public class QAItem
    {
        public string Question { get; set; } = string.Empty;

        public List<string> Answers { get; set; } = new List<string>();
    }
}