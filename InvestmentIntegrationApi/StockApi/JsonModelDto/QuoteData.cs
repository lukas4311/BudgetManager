using Newtonsoft.Json;

namespace BudgetManager.FinanceDataMining.StockApi.JsonModelDto
{
    internal class QuoteData
    {
        [JsonProperty("c")]
        public double CurrentPrice { get; set; }

        [JsonProperty("h")]
        public double HighPrice { get; set; }

        [JsonProperty("l")]
        public double LowPrice { get; set; }

        [JsonProperty("o")]
        public double OpenPrice { get; set; }

        [JsonProperty("pc")]
        public double PreviousClosePrice { get; set; }

        [JsonProperty("t")]
        public int Date { get; set; }
    }
}
