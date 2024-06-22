using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents the state of processing for a broker report.
    /// </summary>
    public class BrokerReportToProcessState : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the broker report state.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code representing the broker report state.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the broker report state.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of broker reports associated with this state.
        /// </summary>
        public List<BrokerReportToProcess> BrokerReportsToProcess { get; set; }
    }
}
