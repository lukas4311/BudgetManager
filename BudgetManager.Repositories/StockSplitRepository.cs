using BudgetManager.Data.DataModels;
using BudgetManager.Data;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IStockSplitRepository" />
    public class StockSplitRepository : Repository<StockSplit>, IStockSplitRepository
    {
        public StockSplitRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
