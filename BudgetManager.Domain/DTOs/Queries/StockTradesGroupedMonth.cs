namespace BudgetManager.Domain.DTOs.Queries
{
    public class StockTradesGroupedMonth
    {
        public int StockTickerId { get; set; }
        public int TradeYear { get; set; }
        public int TradeMonth { get; set; }
        public int TradeSize { get; set; }
        public decimal TradeValue { get; set; }
        public int AccumulatedTradeSize { get; set; }
    }
}
