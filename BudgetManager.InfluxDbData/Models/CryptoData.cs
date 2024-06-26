﻿using InfluxDB.Client.Core;
using System;

namespace BudgetManager.InfluxDbData
{
    [Measurement("cryptoData")]
    public class CryptoData : IInfluxModel
    {
        [Column("closePrice")]
        public double ClosePrice { get; set; }

        [Column("highPrices")]
        public double HighPrice { get; set; }

        [Column("lowPrices")]
        public double LowPrice { get; set; }

        [Column("openPrices")]
        public double OpenPrice { get; set; }

        [Column("volumes")]
        public double Volume { get; set; }

        [Column("ticker", IsTag = true)]
        public string Ticker { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
    }


    [Measurement("Price")]
    public class CryptoDataV2 : IInfluxModel
    {
        [Column("price")]
        public double Price { get; set; }

        [Column("ticker", IsTag = true)]
        public string Ticker { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
    }
}
