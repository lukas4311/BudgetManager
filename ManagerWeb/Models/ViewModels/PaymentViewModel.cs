using System;

namespace ManagerWeb.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public int BankAccountId { get; set; }

        public int PaymentTypeId { get; set; }

        public int PaymentCategoryId { get; set; }
    }
}
