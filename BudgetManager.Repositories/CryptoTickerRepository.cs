using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class CryptoTickerRepository : Repository<CryptoTicker>, ICryptoTickerRepository
    {
        public CryptoTickerRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
