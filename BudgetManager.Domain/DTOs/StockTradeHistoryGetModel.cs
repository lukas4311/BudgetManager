namespace BudgetManager.Domain.DTOs
{
    public class StockTradeHistoryGetModel: StockTradeHistoryModel
    {
        public string CurrencySymbol { get; set; }

        public double TradeSizeAfterAplit { get; set; }
    }
}
