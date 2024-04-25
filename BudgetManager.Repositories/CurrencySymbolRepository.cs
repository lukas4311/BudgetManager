using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="ICurrencySymbolRepository" />
    public class CurrencySymbolRepository : Repository<CurrencySymbol>, ICurrencySymbolRepository
    {
        public CurrencySymbolRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
