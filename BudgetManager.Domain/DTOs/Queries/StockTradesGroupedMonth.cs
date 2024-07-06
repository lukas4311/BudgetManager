namespace BudgetManager.Domain.DTOs.Queries
{
    public record StockTradesGroupedMonth
    {
        public int StockTickerId { get; init; }

        public int TradeYear { get; init; }

        public int TradeMonth { get; init; }

        public double TradeSize { get; init; }

        public double TradeValue { get; init; }

        public double AccumulatedTradeSize { get; init; }
    }
}
