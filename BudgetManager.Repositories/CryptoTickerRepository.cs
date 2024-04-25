using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="ICryptoTickerRepository" />
    public class CryptoTickerRepository : Repository<CryptoTicker>, ICryptoTickerRepository
    {
        public CryptoTickerRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
