namespace BudgetManager.Domain.DTOs.Queries
{
    public record TradeGroupedTicker
    {
        public int TickerId { get; init; }

        public double TotalTradeSize { get; init; }

        public double TotalTradeValue { get; init; }

        public double AccumulatedTradeSize { get; init; }
    }
}
