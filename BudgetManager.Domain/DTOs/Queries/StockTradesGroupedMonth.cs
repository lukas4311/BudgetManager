namespace BudgetManager.Domain.DTOs.Queries
{
    public class StockTradesGroupedMonth
    {
        public int StockTickerId { get; set; }
        public int TradeYear { get; set; }
        public int TradeMonth { get; set; }
        public double TradeSize { get; set; }
        public double TradeValue { get; set; }
        public double AccumulatedTradeSize { get; set; }
    }
}
