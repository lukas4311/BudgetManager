using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IInterestRateRepository" />
    public class InterestRateRepository : Repository<InterestRate>, IInterestRateRepository
    {
        public InterestRateRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
