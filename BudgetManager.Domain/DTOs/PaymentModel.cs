using System;
using System.Collections.Generic;

namespace BudgetManager.Domain.DTOs
{
    public class PaymentModel : IDtoModel
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public int? BankAccountId { get; set; }

        public int? PaymentTypeId { get; set; }

        public int? PaymentCategoryId { get; set; }

        public string PaymentTypeCode { get; set; }

        public string PaymentCategoryIcon { get; set; }

        public string PaymentCategoryCode { get; set; }

        public List<string> Tags { get; set; }
    }
}
