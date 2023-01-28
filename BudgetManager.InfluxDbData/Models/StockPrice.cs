using InfluxDB.Client.Core;
using System;

namespace BudgetManager.InfluxDbData.Models
{
    [Measurement("Price")]
    public class StockPrice : IInfluxModel
    {
        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }

        [Column("ticker")]
        public string Ticker { get; set; }

        [Column("price")]
        public double Price { get; set; }
    }
}
