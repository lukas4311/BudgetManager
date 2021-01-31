using Data;
using Data.DataModels;

namespace Repository
{
    public class CryptoTickerRepository : Repository<CryptoTicker>, ICryptoTickerRepository
    {
        public CryptoTickerRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
