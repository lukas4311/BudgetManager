namespace BudgetManager.Domain.DTOs.Queries
{
    /// <summary>
    /// Model representing a grouped ticker for stock trades.
    /// </summary>
    public record TradeGroupedTicker
    {
        /// <summary>
        /// Gets or sets the ticker identifier.
        /// </summary>
        public int TickerId { get; init; }

        /// <summary>
        /// Gets or sets the total trade size for the ticker.
        /// </summary>
        public double Size { get; init; }

        /// <summary>
        /// Gets or sets the total trade value for the ticker.
        /// </summary>
        public double Value { get; init; }

        /// <summary>
        /// Gets or sets the accumulated trade size up to this point.
        /// </summary>
        public double AccumulatedSize { get; init; }

        /// <summary>
        /// Gets or sets the identifier for the trade currency symbol.
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

    public record TradeGroupedTickerWithProfitLoss : TradeGroupedTicker
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
