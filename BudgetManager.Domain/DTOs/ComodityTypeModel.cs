using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class ComodityTypeModel : IDtoModel<ComodityType>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int ComodityUnitId { get; set; }

        public string ComodityUnit { get; set; }

        public ComodityType ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}
