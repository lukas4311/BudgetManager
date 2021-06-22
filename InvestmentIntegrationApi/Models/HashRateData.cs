using System.Text.Json.Serialization;

namespace BudgetManager.FinanceDataMining.Models
{
    public class HashRateData
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }
    }
}
