namespace BudgetManager.Domain.DTOs.Queries
{
    public record ComodityTradeGroupedTicker
    {
        public int ComodityTypeId { get; init; }

        public double TotalTradeSize { get; init; }

        public double TotalTradeValue { get; init; }
    }
}
