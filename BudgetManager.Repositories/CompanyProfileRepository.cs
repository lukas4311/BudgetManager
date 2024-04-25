using BudgetManager.Data.DataModels;
using BudgetManager.Data;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="ICompanyProfileRepository" />
    internal class CompanyProfileRepository : Repository<CompanyProfile>, ICompanyProfileRepository
    {
        public CompanyProfileRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
