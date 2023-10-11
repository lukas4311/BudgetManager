using System;

namespace BudgetManager.Data.DataModels
{
    public class StockSplit : IDataModel
    {
        public int Id { get; set; }

        public int StockTickerId { get; set; }

        public StockTicker StockTicker { get; set; }

        public DateTime SplitTimeStamp { get; set; }

        public string SplitTextInfo { get; set; }

        public double SplitCoefficient { get; set; }
    }
}
