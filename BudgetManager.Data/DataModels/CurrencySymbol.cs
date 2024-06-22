using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a currency symbol used in various trade histories and investments.
    /// </summary>
    public class CurrencySymbol : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the currency symbol.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the symbol representing the currency.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the collection of cryptocurrency trade histories associated with this currency symbol.
        /// </summary>
        public List<CryptoTradeHistory> CryptoTradeHistory { get; set; }

        /// <summary>
        /// Gets or sets the collection of commodity trade histories associated with this currency symbol.
        /// </summary>
        public List<ComodityTradeHistory> ComodityTradeHistory { get; set; }

        /// <summary>
        /// Gets or sets the collection of other investments associated with this currency symbol.
        /// </summary>
        public IEnumerable<OtherInvestment> OtherInvestments { get; set; }
    }
}
