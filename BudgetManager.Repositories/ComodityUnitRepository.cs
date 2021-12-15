using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class ComodityUnitRepository : Repository<ComodityUnit>, IComodityUnitRepository
    {
        public ComodityUnitRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}