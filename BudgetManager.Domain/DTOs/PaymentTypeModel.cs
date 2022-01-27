using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class PaymentTypeModel : IDtoModel<PaymentType>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public PaymentType ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}