using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IBrokerReportToProcessRepository" />
    public class BrokerReportToProcessRepository : Repository<BrokerReportToProcess>, IBrokerReportToProcessRepository
    {
        public BrokerReportToProcessRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}