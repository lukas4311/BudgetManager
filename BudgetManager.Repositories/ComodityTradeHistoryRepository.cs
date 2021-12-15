using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class ComodityTradeHistoryRepository : Repository<ComodityTradeHistory>, IComodityTradeHistoryRepository
    {
        public ComodityTradeHistoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}