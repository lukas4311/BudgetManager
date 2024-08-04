namespace BudgetManager.Domain.DTOs.Queries
{
    public class TradesGroupedMonth
    {
        public int TickerId { get; init; }

        public int TradeYear { get; init; }

        public int TradeMonth { get; init; }

        public double TradeSize { get; init; }

        public double TradeValue { get; init; }

        public double AccumulatedTradeSize { get; init; }
    }
}
