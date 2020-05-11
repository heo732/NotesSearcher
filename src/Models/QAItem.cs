using Newtonsoft.Json;

namespace QAHelper.Models
{
    public class QAItem
    {
        [JsonProperty("Q")]
        public string Question { get; set; }

        [JsonProperty("A")]
        public string Answer { get; set; }
    }
}