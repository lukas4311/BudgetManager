using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IComodityTypeRepository" />
    public class ComodityTypeRepository : Repository<ComodityType>, IComodityTypeRepository
    {
        public ComodityTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}