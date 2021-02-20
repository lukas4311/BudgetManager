using InfluxDB.Client.Core;
using System;

namespace InfluxDbData
{
    [Measurement("forexData")]
    public class ForexData : IInfluxModel
    {
        [Column("price")]
        public double Price { get; set; }

        [Column("currency", IsTag = true)]
        public string Currency { get; set; }

        [Column("baseCurrency", IsTag = true)]
        public string BaseCurrency { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
    }
}
