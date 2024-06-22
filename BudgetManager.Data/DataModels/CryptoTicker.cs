using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a cryptocurrency ticker symbol.
    /// </summary>
    public class CryptoTicker : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the cryptocurrency ticker.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ticker symbol of the cryptocurrency.
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Gets or sets the name of the cryptocurrency.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of trade histories associated with this cryptocurrency ticker.
        /// </summary>
        public IList<CryptoTradeHistory> CryptoTradeHistories { get; set; }
    }
}
