using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a bank account associated with a user.
    /// </summary>
    public class BankAccount : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the bank account.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code or identifier of the bank account.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who owns the bank account.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user associated with this bank account.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }

        /// <summary>
        /// Gets or sets the list of payments associated with this bank account.
        /// </summary>
        public List<Payment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the list of interest rates associated with this bank account.
        /// </summary>
        public List<InterestRate> InterestRates { get; set; }

        /// <summary>
        /// Gets or sets the opening balance of the bank account.
        /// </summary>
        public int OpeningBalance { get; set; }
    }
}
