using Newtonsoft.Json;
using System.Collections.Generic;

namespace BudgetManager.FinanceDataMining.StockApi.JsonModelDto
{
    internal class CandleData
    {
        [JsonProperty("c")]
        public List<double> ClosePrices { get; set; }
        [JsonProperty("h")]
        public List<double> HighPrices { get; set; }
        [JsonProperty("l")]
        public List<double> LowPrices { get; set; }
        [JsonProperty("o")]
        public List<double> OpenPrices { get; set; }
        [JsonProperty("s")]
        public string Status { get; set; }
        [JsonProperty("t")]
        public List<int> Dates { get; set; }
        [JsonProperty("v")]
        public List<int> Volumes { get; set; }
    }
}
