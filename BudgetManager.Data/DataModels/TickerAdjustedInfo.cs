using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BudgetManager.Data.DataModels
{
    public class TickerAdjustedInfo : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the stock trade history entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the price ticker associated with the stock.
        /// </summary>
        public string PriceTicker { get; set; }

        /// <summary>
        /// Gets or sets the company information ticker associated with the stock.
        /// </summary>
        public string CompanyInfoTicker { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        public string Metadata { get; set; }
    }
}
