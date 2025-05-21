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
        public DateTime TimeStamp { get; init; }

        /// <summary>
        /// Gets or sets the total trade size.
        /// </summary>
        public double Size { get; init; }

        /// <summary>
        /// Gets or sets the total trade value.
        /// </summary>
        public double Value { get; init; }

        /// <summary>
        /// Gets or sets the accumulated trade size.
        /// </summary>
        public double AccumulatedSize { get; init; }

        /// <summary>
        /// Gets or sets Id of currency.
        /// </summary>
        public int CurrencySymbolId { get; init; }

        /// <summary>
        /// Gets or sets the code for the ticker.
        /// </summary>
        public string TickerCode { get; init; }

        /// <summary>
        /// Gets or sets the code for the currency.
        /// </summary>
        public string CurrencyCode { get; init; }
    }

    public record TradeGroupedTradeTimeWithProfitLoss : TradeGroupedTradeTime
    {
        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public double TotalAccumulatedValue { get; init; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public double TotalPercentageProfitOrLoss { get; init; }
    }
}