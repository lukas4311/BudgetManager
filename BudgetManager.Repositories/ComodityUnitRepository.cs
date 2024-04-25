using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IComodityUnitRepository" />
    public class ComodityUnitRepository : Repository<ComodityUnit>, IComodityUnitRepository
    {
        public ComodityUnitRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}