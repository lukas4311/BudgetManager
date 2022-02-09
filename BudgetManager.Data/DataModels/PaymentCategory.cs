using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class PaymentCategory : IDataModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<Payment> Payments { get; set; }

        public string Icon { get; set; }
    }
}
