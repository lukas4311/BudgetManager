namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a tag associated with a payment.
    /// </summary>
    public class PaymentTag : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the payment tag.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the payment associated with this tag.
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// Gets or sets the payment associated with this tag.
        /// </summary>
        public Payment Payment { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the tag associated with this payment.
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// Gets or sets the tag associated with this payment.
        /// </summary>
        public Tag Tag { get; set; }
    }
}
