using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a category for payments.
    /// </summary>
    public class PaymentCategory : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the payment category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code associated with the payment category.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the payment category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of payments associated with this payment category.
        /// </summary>
        public List<Payment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the icon associated with the payment category.
        /// </summary>
        public string Icon { get; set; }
    }
}
