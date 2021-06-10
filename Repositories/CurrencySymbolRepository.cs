using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace Repository
{
    public class CurrencySymbolRepository : Repository<CurrencySymbol>, ICurrencySymbolRepository
    {
        public CurrencySymbolRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
