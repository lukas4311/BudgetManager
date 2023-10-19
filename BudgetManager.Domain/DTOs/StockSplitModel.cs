using System;

namespace BudgetManager.Domain.DTOs
{
    public class StockSplitModel : IDtoModel
    {
        public int? Id { get; set; }

        public int StockTickerId { get; set; }

        public DateTime SplitTimeStamp { get; set; }

        public string SplitTextInfo { get; set; }

        public double SplitCoefficient { get; set; }
    }

    public class StockSplitAccumulated
    {
        //int? Id, int StockTickerId, DateTime SpliDateTime,
        //double SplitAccumulatedCoeficient) 
        public int? Id { get; set; }

        public int StockTickerId { get; set; }

        public DateTime SpliDateTime { get; set; }

        public double SplitAccumulatedCoeficient { get; set; }
    }
}
