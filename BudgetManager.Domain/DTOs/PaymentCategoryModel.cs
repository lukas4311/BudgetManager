using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class PaymentCategoryModel : IDtoModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }
    }
}
