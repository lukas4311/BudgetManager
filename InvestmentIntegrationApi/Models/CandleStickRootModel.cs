using Newtonsoft.Json;

namespace FinanceDataMining.Models
{
    internal class CandleStickRootModel
    {
        [JsonProperty("result")]
        public CandleStickData CandleStickData { get; set; }
    }
}
