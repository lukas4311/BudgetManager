namespace BudgetManager.Domain.DTOs.Queries
{
    /// <summary>
    /// Represents aggregated trade data for a specific ticker in a given month and year.
    /// </summary>
    public class TradesGroupedMonth
    {
        /// <summary>
        /// Gets or sets the ticker identifier.
        /// </summary>
        public int TickerId { get; init; }

        /// <summary>
        /// Gets or sets the year in which the trades occurred.
        /// </summary>
        public int TradeYear { get; init; }

        /// <summary>
        /// Gets or sets the month in which the trades occurred.
        /// </summary>
        public int TradeMonth { get; init; }

        /// <summary>
        /// Gets or sets the total trade size for the specified month.
        /// </summary>
        public double TradeSize { get; init; }

        /// <summary>
        /// Gets or sets the total trade value for the specified month.
        /// </summary>
        public double TradeValue { get; init; }

        /// <summary>
        /// Gets or sets the accumulated trade size up to and including the specified month.
        /// </summary>
        public double AccumulatedTradeSize { get; init; }

        /// <summary>
        /// Gets or sets the identifier for the trade currency symbol.
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