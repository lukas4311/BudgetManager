using System.Text.Json.Serialization;

namespace BudgetManager.FinanceDataMining.Models
{
    public class FearAndGreedResponseData
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("value_classification")]
        public string ValueClassification { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("time_until_update")]
        public string TimeUntilUpdate { get; set; }
    }
}
