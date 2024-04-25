using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IOtherInvestmentRepository" />
    public class OtherInvestmentRepository : Repository<OtherInvestment>, IOtherInvestmentRepository
    {
        public OtherInvestmentRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
