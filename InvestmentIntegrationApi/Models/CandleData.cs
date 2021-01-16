using Newtonsoft.Json;
using System;

namespace FinanceDataMining.Models
{
    public class CandleModel
    {
        [JsonProperty("timestamp")]
        public string DateTime { get; set; }

        [JsonProperty("open")]
        public double Open { get; set; }

        [JsonProperty("high")]
        public double High { get; set; }

        [JsonProperty("low")]
        public double Low { get; set; }

        [JsonProperty("close")]
        public double Close { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }
    }
}
