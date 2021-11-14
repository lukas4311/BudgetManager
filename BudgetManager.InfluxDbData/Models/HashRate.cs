using System;
using InfluxDB.Client.Core;

namespace BudgetManager.InfluxDbData.Models
{
    [Measurement("hashRate")]
    public class HashRate : IInfluxModel
    {
        [Column("value")]
        public double Value { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
    }
}
