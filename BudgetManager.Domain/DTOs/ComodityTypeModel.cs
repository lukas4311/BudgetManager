namespace BudgetManager.Domain.DTOs
{
    public class ComodityTypeModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int ComodityUnitId { get; set; }

        public string ComodityUnit { get; set; }
    }
}
