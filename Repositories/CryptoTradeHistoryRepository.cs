using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace Repository
{
    public class CryptoTradeHistoryRepository : Repository<CryptoTradeHistory>, ICryptoTradeHistoryRepository
    {
        public CryptoTradeHistoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
