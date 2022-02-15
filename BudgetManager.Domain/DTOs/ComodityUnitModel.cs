namespace BudgetManager.Domain.DTOs
{
    public class ComodityUnitModel : IDtoModel
    {
        public int? Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}
