using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class InterestRateRepository : Repository<InterestRate>, IInterestRateRepository
    {
        public InterestRateRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
