using System;

namespace BudgetManager.Domain.DTOs.Queries
{
    public record StockTradeGroupedTradeTime
    {
        public int StockTickerId { get; init; }

        public DateTime TradeTimeStamp { get; init; }

        public double TotalTradeSize { get; init; }

        public double TotalTradeValue { get; init; }

        public double AccumulatedTradeSize { get; init; }
    }
}
