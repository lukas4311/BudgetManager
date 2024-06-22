using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a type of payment.
    /// </summary>
    public class PaymentType : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the payment type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code of the payment type.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the payment type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of payments associated with this payment type.
        /// </summary>
        public List<Payment> Payments { get; set; }
    }
}
