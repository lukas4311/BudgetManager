using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class BrokerReportToProcessRepository : Repository<BrokerReportToProcess>, IBrokerReportToProcessRepository
    {
        public BrokerReportToProcessRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}