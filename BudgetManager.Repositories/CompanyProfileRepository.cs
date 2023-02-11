using BudgetManager.Data.DataModels;
using BudgetManager.Data;

namespace BudgetManager.Repository
{
    internal class CompanyProfileRepository : Repository<CompanyProfile>, ICompanyProfileRepository
    {
        public CompanyProfileRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
