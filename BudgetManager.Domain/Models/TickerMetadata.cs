using System.Text.Json.Serialization;

namespace BudgetManager.Domain.Models
{
    public class TickerMetadata
    {
            [JsonPropertyName("isin")]
            public string Isin { get; set; }

            [JsonPropertyName("figi")]
            public object Figi { get; set; }

            [JsonPropertyName("short_name")]
            public string ShortName { get; set; }

            [JsonPropertyName("short_description")]
            public string ShortDescription { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            [JsonPropertyName("resolved_symbol")]
            public string Resolved_symbol { get; set; }

            [JsonPropertyName("exchange")]
            public string Exchange { get; set; }

            [JsonPropertyName("pro_symbol")]
            public string ProSymbol { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("price_ticker")]
            public string PriceTicker { get; set; }
    }
}
