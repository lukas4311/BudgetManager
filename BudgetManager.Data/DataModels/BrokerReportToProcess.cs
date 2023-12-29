namespace BudgetManager.Data.DataModels
{
    public class BrokerReportToProcess : IDataModel
    {
        public int Id { get; set; }

        public string FileContentBase64 { get; set; }

        public int BrokerReportToProcessStateId { get; set; }

        public BrokerReportToProcessState BrokerReportToProcessState { get; set; }
    }
}
