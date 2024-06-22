using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a tag used for categorizing payments and other investments.
    /// </summary>
    public class Tag : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tag.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code associated with the tag.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the list of payment tags associated with this tag.
        /// </summary>
        public IList<PaymentTag> PaymentTags { get; set; }

        /// <summary>
        /// Gets or sets the list of other investment tags associated with this tag.
        /// </summary>
        public IEnumerable<OtherInvestmentTag> OtherInvestmentTags { get; set; }
    }
}
