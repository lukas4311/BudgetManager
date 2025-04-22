using System;

namespace BudgetManager.Domain.DTOs.Queries
{
    /// <summary>
    /// Model representing a grouped trade time for a stock trade.
    /// </summary>
    public record TradeGroupedTradeTime
    {
        /// <summary>
        /// Gets or sets the identifier of the stock trade.
        /// </summary>
        public int TickerId { get; init; }

        /// <summary>
        /// Gets or sets the trade time stamp.
        /// </summary>
        public DateTime TradeTimeStamp { get; init; }

        /// <summary>
        /// Gets or sets the total trade size.
        /// </summary>
        public double TotalTradeSize { get; init; }

        /// <summary>
        /// Gets or sets the total trade value.
        /// </summary>
        public double TotalTradeValue { get; init; }

        /// <summary>
        /// Gets or sets the accumulated trade size.
        /// </summary>
        public double AccumulatedTradeSize { get; init; }

        /// <summary>
        /// Gets or sets Id of currency.
        /// </summary>
        public int TradeCurrencySymbolId { get; init; }

        /// <summary>
        /// Gets or sets the code for the ticker.
        /// </summary>
        public string TickerCode { get; init; }

        /// <summary>
        /// Gets or sets the code for the currency.
        /// </summary>
        public string CurrencyCode { get; init; }
    }
}
