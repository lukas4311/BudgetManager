using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class BrokerReportToProcessState : IDataModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<BrokerReportToProcess> BrokerReportsToProcess { get; set; }
    }
}
