using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents the balance history of an other investment.
    /// </summary>
    public class OtherInvestmentBalaceHistory : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the balance history entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the balance was recorded.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the balance amount at the recorded date.
        /// </summary>
        public int Balance { get; set; }

        /// <summary>
        /// Gets or sets the ID of the other investment associated with this balance history.
        /// </summary>
        public int OtherInvestmentId { get; set; }

        /// <summary>
        /// Gets or sets the other investment entity associated with this balance history.
        /// </summary>
        public OtherInvestment OtherInvestment { get; set; }
    }
}
