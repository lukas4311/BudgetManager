using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents an interest rate associated with a bank account.
    /// </summary>
    public class InterestRate : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the interest rate.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the lower bound of the range for which this interest rate applies.
        /// </summary>
        public decimal RangeFrom { get; set; }

        /// <summary>
        /// Gets or sets the optional upper bound of the range for which this interest rate applies.
        /// </summary>
        public decimal? RangeTo { get; set; }

        /// <summary>
        /// Gets or sets the value of the interest rate.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the ID of the bank account associated with this interest rate.
        /// </summary>
        public int BankAccountId { get; set; }

        /// <summary>
        /// Gets or sets the bank account entity associated with this interest rate.
        /// </summary>
        public BankAccount BankAccount { get; set; }

        /// <summary>
        /// Gets or sets the date when the interest rate payout occurs.
        /// </summary>
        public DateTime PayoutDate { get; set; }
    }
}
