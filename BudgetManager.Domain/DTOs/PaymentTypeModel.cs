namespace BudgetManager.Domain.DTOs
{
    public class PaymentTypeModel : IDtoModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }
    }
}