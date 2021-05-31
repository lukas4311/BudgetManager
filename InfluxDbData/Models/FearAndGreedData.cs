using InfluxDB.Client.Core;
using System;

namespace InfluxDbData.Models
{
    [Measurement("fearAndGreed")]
    public class FearAndGreedData : IInfluxModel
    {
        [Column("value")]
        public double Value { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
    }
}
