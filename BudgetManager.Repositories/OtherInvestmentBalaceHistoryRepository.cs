using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class OtherInvestmentBalaceHistoryRepository : Repository<OtherInvestmentBalaceHistory>, IOtherInvestmentBalaceHistoryRepository
    {
        public OtherInvestmentBalaceHistoryRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
