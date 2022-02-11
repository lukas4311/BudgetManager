using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class OtherInvestmentRepository : Repository<OtherInvestment>, IOtherInvestmentRepository
    {
        public OtherInvestmentRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
