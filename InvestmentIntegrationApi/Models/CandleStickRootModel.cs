using Newtonsoft.Json;

namespace BudgetManager.FinanceDataMining.Models
{
    internal class CandleStickRootModel
    {
        [JsonProperty("result")]
        public CandleStickData CandleStickData { get; set; }
    }
}
