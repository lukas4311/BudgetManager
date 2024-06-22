using System;
using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a payment made from a bank account.
    /// </summary>
    public class Payment : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the payment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the amount of the payment.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the name or title of the payment.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description or details of the payment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date when the payment was made.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the ID of the bank account from which the payment was made.
        /// </summary>
        public int BankAccountId { get; set; }

        /// <summary>
        /// Gets or sets the bank account entity associated with this payment.
        /// </summary>
        public BankAccount BankAccount { get; set; }

        /// <summary>
        /// Gets or sets the ID of the payment type (e.g., credit, debit, transfer).
        /// </summary>
        public int PaymentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the payment type entity associated with this payment.
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Gets or sets the ID of the payment category (e.g., utilities, groceries).
        /// </summary>
        public int PaymentCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the payment category entity associated with this payment.
        /// </summary>
        public PaymentCategory PaymentCategory { get; set; }

        /// <summary>
        /// Gets or sets the list of payment tags associated with this payment.
        /// </summary>
        public IList<PaymentTag> PaymentTags { get; set; }
    }
}
