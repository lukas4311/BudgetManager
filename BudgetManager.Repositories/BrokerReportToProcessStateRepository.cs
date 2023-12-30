using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    public class BrokerReportToProcessStateRepository : Repository<BrokerReportToProcessState>, IBrokerReportToProcessStateRepository
    {
        public BrokerReportToProcessStateRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}