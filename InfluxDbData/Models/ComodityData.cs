using InfluxDB.Client.Core;
using System;

namespace InfluxDbData
{
    [Measurement("comodityData")]
    public class ComodityData : IInfluxModel
    {
        [Column("price")]
        public double Price { get; set; }

        [Column("type", IsTag = true)]
        public string Ticker { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
    }
}
