using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a commodity trade history entity.
    /// </summary>
    public class ComodityTradeHistory : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the commodity trade history.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the trade occurred.
        /// </summary>
        public DateTime TradeTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the commodity type associated with the trade.
        /// </summary>
        public int ComodityTypeId { get; set; }

        /// <summary>
        /// Gets or sets the commodity type associated with the trade.
        /// </summary>
        public ComodityType ComodityType { get; set; }

        /// <summary>
        /// Gets or sets the size of the trade.
        /// </summary>
        public double TradeSize { get; set; }

        /// <summary>
        /// Gets or sets the value of the trade.
        /// </summary>
        public double TradeValue { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the currency symbol used for the trade.
        /// </summary>
        public int CurrencySymbolId { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol used for the trade.
        /// </summary>
        public CurrencySymbol CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user associated with the trade.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the trade.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }

        /// <summary>
        /// Gets or sets the name of the company involved in the trade.
        /// </summary>
        public string Company { get; set; }
    }
}
