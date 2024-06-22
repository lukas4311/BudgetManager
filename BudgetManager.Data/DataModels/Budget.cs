using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a budget entity.
    /// </summary>
    public class Budget : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the budget.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the starting date of the budget period.
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Gets or sets the ending date of the budget period.
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Gets or sets the amount allocated for the budget.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Gets or sets the name or description of the budget.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user associated with the budget.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the budget.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }
    }
}
