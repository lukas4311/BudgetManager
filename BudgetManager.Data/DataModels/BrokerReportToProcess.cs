using System;

namespace BudgetManager.Data.DataModels
{
    public class BrokerReportToProcess : IDataModel
    {
        public int Id { get; set; }

        public DateTime ImportedTime { get; set; }

        public string FileContentBase64 { get; set; }

        public int BrokerReportToProcessStateId { get; set; }

        public BrokerReportToProcessState BrokerReportToProcessState { get; set; }

        public int UserIdentityId { get; set; }

        public UserIdentity UserIdentity { get; set; }

        public int BrokerReportTypeId { get; set; }

        public BrokerReportType BrokerReportType { get; set; }

    }
}
