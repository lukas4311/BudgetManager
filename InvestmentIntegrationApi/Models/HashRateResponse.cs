using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BudgetManager.FinanceDataMining.Models
{
    public class HashRateResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("values")]
        public List<HashRateData> Values { get; set; }
    }
}
