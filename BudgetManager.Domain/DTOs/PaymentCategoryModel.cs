using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class PaymentCategoryModel : IDtoModel<PaymentCategory>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public PaymentCategory ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}
