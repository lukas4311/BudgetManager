using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="ICryptoTradeHistoryRepository" />
    public class CryptoTradeHistoryRepository : Repository<CryptoTradeHistory>, ICryptoTradeHistoryRepository
    {
        public CryptoTradeHistoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
