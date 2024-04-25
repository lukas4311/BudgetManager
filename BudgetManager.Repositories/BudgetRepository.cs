using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IBudgetRepository" />
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        public BudgetRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
