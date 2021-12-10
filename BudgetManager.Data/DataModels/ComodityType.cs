namespace BudgetManager.Data.DataModels
{
    public class ComodityType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ComodityUnit ComodityUnit { get; set; }

        public int ComodityUnitId { get; set; }
    }
}
