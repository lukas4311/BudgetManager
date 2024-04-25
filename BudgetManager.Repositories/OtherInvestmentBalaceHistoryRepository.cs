using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IOtherInvestmentBalaceHistoryRepository" />
    public class OtherInvestmentBalaceHistoryRepository : Repository<OtherInvestmentBalaceHistory>, IOtherInvestmentBalaceHistoryRepository
    {
        public OtherInvestmentBalaceHistoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
