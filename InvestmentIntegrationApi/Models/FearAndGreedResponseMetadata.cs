using System.Text.Json.Serialization;

namespace FinanceDataMining.Models
{
    public class FearAndGreedResponseMetadata
    {
        [JsonPropertyName("error")]
        public object Error { get; set; }
    }
}
