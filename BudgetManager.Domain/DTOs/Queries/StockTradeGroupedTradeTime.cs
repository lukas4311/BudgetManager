using System;

namespace BudgetManager.Domain.DTOs.Queries
{
    public record TradeGroupedTradeTime
    {
        public int TickerId { get; init; }

        public DateTime TradeTimeStamp { get; init; }

        public double TotalTradeSize { get; init; }

        public double TotalTradeValue { get; init; }

        public double AccumulatedTradeSize { get; init; }
    }
}
