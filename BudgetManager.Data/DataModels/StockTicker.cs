namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a stock ticker symbol and its associated name.
    /// </summary>
    public class StockTicker : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the stock ticker.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ticker symbol of the stock.
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Gets or sets the name of the stock.
        /// </summary>
        public string Name { get; set; }
    }
}
