﻿using System;

namespace BudgetManager.FinanceDataMining.StockApi.Models
{
    public class StockData
    {
        public double CurrentPrice { get; set; }

        public double HighPrice { get; set; }

        public double LowPrice { get; set; }

        public double OpenPrice { get; set; }

        public double PreviousClosePrice { get; set; }

        public DateTime Date { get; set; }
    }
}
