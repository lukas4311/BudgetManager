using InfluxDB.Client.Core;
using System;

namespace BudgetManager.InfluxDbData
{
    [Measurement("stockData")]
    public class StockData : IInfluxModel
    {
        [Column("closePrice")]
        public double ClosePrice { get; set; }

        [Column("highPrices")]
        public double HighPrice { get; set; }

        [Column("lowPrices")]
        public double LowPrice { get; set; }

        [Column("openPrices")]
        public double OpenPrice { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("volumes")]
        public double Volume { get; set; }

        [Column("ticker", IsTag = true)]
        public string Ticker { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
    }
}
