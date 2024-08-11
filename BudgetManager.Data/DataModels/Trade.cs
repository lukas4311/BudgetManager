using System;

namespace BudgetManager.Data.DataModels
{
    public class Trade : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the stock trade history entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the trade occurred.
        /// </summary>
        public DateTime TradeTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated stock ticker.
        /// </summary>
        public int TickerId { get; set; }

        /// <summary>
        /// Gets or sets the associated stock ticker object.
        /// </summary>
        public EnumItem Ticker { get; set; }

        /// <summary>
        /// Gets or sets the size of the trade (e.g., number of shares).
        /// </summary>
        public double TradeSize { get; set; }

        /// <summary>
        /// Gets or sets the value of the trade.
        /// </summary>
        public double TradeValue { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the currency symbol used for the trade.
        /// </summary>
        public int TradeCurrencySymbolId { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol object used for the trade.
        /// </summary>
        public EnumItem TradeCurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user associated with the trade.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user identity object associated with the trade.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }

        /// <summary>
        /// Trade metadata
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// External id of transaction
        /// </summary>
        public string TransactionId { get; set; }
    }
}
