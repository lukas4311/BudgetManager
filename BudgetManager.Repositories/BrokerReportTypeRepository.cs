using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IBrokerReportTypeRepository" />
    public class BrokerReportTypeRepository : Repository<BrokerReportType>, IBrokerReportTypeRepository
    {
        public BrokerReportTypeRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}