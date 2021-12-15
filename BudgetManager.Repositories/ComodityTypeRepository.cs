using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class ComodityTypeRepository : Repository<ComodityType>, IComodityTypeRepository
    {
        public ComodityTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}