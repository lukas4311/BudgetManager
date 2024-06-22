using System;
using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents an other investment held by a user.
    /// </summary>
    public class OtherInvestment : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the other investment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the other investment was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the code representing the other investment.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the other investment.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the opening balance of the other investment.
        /// </summary>
        public int OpeningBalance { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who owns the other investment.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user entity who owns the other investment.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }

        /// <summary>
        /// Gets or sets the ID of the currency symbol associated with the other investment.
        /// </summary>
        public int CurrencySymbolId { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol entity associated with the other investment.
        /// </summary>
        public CurrencySymbol CurrencySymbol { get; set; }

        /// <summary>
        /// Gets or sets the collection of balance history entries for the other investment.
        /// </summary>
        public IEnumerable<OtherInvestmentBalaceHistory> OtherInvestmentBalaceHistory { get; set; }

        /// <summary>
        /// Gets or sets the collection of tags associated with the other investment.
        /// </summary>
        public IEnumerable<OtherInvestmentTag> OtherInvestmentTags { get; set; }
    }
}
