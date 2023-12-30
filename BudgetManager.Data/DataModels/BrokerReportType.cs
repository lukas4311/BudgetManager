using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    public class BrokerReportType
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public List<BrokerReportToProcess> BrokerReportsToProcess { get; set; }
    }
}
