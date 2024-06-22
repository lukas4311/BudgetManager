using System;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a broker report to be processed in the system.
    /// </summary>
    public class BrokerReportToProcess : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the broker report.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the report was imported.
        /// </summary>
        public DateTime ImportedTime { get; set; }

        /// <summary>
        /// Gets or sets the base64-encoded content of the broker report file.
        /// </summary>
        public string FileContentBase64 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the state of processing for the broker report.
        /// </summary>
        public int BrokerReportToProcessStateId { get; set; }

        /// <summary>
        /// Gets or sets the state of processing for the broker report.
        /// </summary>
        public BrokerReportToProcessState BrokerReportToProcessState { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who imported the broker report.
        /// </summary>
        public int UserIdentityId { get; set; }

        /// <summary>
        /// Gets or sets the user who imported the broker report.
        /// </summary>
        public UserIdentity UserIdentity { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the type of broker report.
        /// </summary>
        public int BrokerReportTypeId { get; set; }

        /// <summary>
        /// Gets or sets the type of broker report.
        /// </summary>
        public BrokerReportType BrokerReportType { get; set; }
    }
}
