using Data;
using Data.DataModels;

namespace Repository
{
    public class CurrencySymbolRepository : Repository<CurrencySymbol>, ICurrencySymbolRepository
    {
        public CurrencySymbolRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
