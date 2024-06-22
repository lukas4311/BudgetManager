using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a historical record of cryptocurrency trades.
    /// </summary>
    public class CryptoTradeHistory : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the cryptocurrency trade history.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the cryptocurrency trade occurred.
        /// </summary>
        public DateTime TradeTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference to the associated cryptocurrency ticker.
        /// </summary>
        public int CryptoTickerId { get; set; }

        /// <summary>
        /// Gets or sets the associated cryptocurrency ticker.
        /// </summary>
        public CryptoTicker CryptoTicker { get; set; }

        /// <summary>
        /// Gets or sets the size of the cryptocurrency trade.
        /// </summary>
        public double TradeSize { get; set; }

        /// <summary>
        /// Gets or sets the value of the cryptocurrency trade.
        /// </summary>
        public double TradeValue { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference to the associated currency symbol used in the trade.
        /// </summary>
        public int CurrencySymbolId { get; set; }

        /// <summary>
        /// Gets or sets the associated currency symbol used in the trade.
        /// </summary>
        public CurrencySymbol CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the foreign key reference to the user associated with the trade.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the trade.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }
    }
}
