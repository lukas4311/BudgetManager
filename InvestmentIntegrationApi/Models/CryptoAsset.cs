using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinanceDataMining.Models
{
    public class CryptoAsset
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("fiat")]
        public bool Fiat { get; set; }

        [JsonPropertyName("route")]
        public string Route { get; set; }
    }

    public class CryptoAssetRoot
    {
        [JsonPropertyName("result")]
        public IEnumerable<CryptoAsset> Assets { get; set; }
    }
}
