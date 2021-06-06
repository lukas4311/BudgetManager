using System.Text.Json.Serialization;

namespace BudgetManager.FinanceDataMining.Models
{
    public class FearAndGreedResponseMetadata
    {
        [JsonPropertyName("error")]
        public object Error { get; set; }
    }
}
