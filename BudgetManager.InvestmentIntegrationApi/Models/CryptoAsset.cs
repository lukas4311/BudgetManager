using Newtonsoft.Json;
using System.Collections.Generic;

namespace BudgetManager.FinanceDataMining.Models
{
    public class CryptoAsset
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fiat")]
        public bool Fiat { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }
    }

    public class CryptoAssetRoot
    {
        [JsonProperty("result")]
        public IEnumerable<CryptoAsset> Assets { get; set; }
    }
}
