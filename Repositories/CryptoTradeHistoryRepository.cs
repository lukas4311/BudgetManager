using Data;
using Data.DataModels;

namespace Repository
{
    public class CryptoTradeHistoryRepository : Repository<CryptoTradeHistory>, ICryptoTradeHistoryRepository
    {
        public CryptoTradeHistoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
