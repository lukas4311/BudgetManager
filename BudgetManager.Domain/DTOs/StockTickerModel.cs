namespace BudgetManager.Domain.DTOs
{
    public class StockTickerModel : IDtoModel
    {
        public int? Id { get; set; }

        public string Ticker { get; set; }

        public string Name { get; set; }
    }
}
