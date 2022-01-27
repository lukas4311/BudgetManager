using BudgetManager.Data.DataModels;

namespace BudgetManager.Domain.DTOs
{
    public class ComodityUnitModel : IDtoModel<ComodityUnit>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public ComodityUnit ToEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}
