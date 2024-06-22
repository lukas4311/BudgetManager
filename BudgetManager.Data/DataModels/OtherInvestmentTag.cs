using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a tag associated with an other investment.
    /// </summary>
    public class OtherInvestmentTag : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the other investment tag.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the other investment associated with this tag.
        /// </summary>
        public int OtherInvestmentId { get; set; }

        /// <summary>
        /// Gets or sets the other investment entity associated with this tag.
        /// </summary>
        public OtherInvestment OtherInvestment { get; set; }

        /// <summary>
        /// Gets or sets the ID of the tag associated with this other investment.
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// Gets or sets the tag entity associated with this other investment.
        /// </summary>
        public Tag Tag { get; set; }
    }
}
