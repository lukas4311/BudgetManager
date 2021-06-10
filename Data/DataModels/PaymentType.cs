using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class PaymentType
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<Payment> Payments { get; set; }
    }
}
