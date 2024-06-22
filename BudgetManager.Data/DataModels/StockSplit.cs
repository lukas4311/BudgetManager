using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a stock split event.
    /// </summary>
    public class StockSplit : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the stock split.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the stock ticker associated with the split.
        /// </summary>
        public int StockTickerId { get; set; }

        /// <summary>
        /// Gets or sets the stock ticker associated with the split.
        /// </summary>
        public StockTicker StockTicker { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the stock split occurred.
        /// </summary>
        public DateTime SplitTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets additional textual information about the stock split.
        /// </summary>
        public string SplitTextInfo { get; set; }

        /// <summary>
        /// Gets or sets the coefficient of the stock split.
        /// </summary>
        public double SplitCoefficient { get; set; }
    }
}
