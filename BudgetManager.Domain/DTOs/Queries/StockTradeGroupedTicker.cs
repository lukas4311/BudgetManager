namespace BudgetManager.Domain.DTOs.Queries
{
    public record StockTradeGroupedTicker
    {
        public int StockTickerId { get; init; }

        public double TotalTradeSize { get; init; }

        public double TotalTradeValue { get; init; }

        public double AccumulatedTradeSize { get; init; }
    }
}
