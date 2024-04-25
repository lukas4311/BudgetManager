using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IStockTickerRepository" />
    public class StockTickerRepository : Repository<StockTicker>, IStockTickerRepository
    {
        public StockTickerRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
