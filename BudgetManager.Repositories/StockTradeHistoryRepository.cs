using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class StockTradeHistoryRepository : Repository<StockTradeHistory>, IStockTradeHistoryRepository
    {
        public StockTradeHistoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
