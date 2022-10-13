namespace BudgetManager.Data.DataModels
{
    public class StockTicker : IDataModel
    {
        public int Id { get; set; }

        public string Ticker { get; set; }
    }
}
