namespace BudgetManager.Domain.DTOs.Queries
{
    public class ComodityTradesGroupedMonth
    {
        public int ComodityTypeId { get; init; }

        public int TradeYear { get; init; }

        public int TradeMonth { get; init; }

        public double AccumulatedTradeValue { get; init; }

        public double AccumulatedTradeSize { get; init; }
    }
}
