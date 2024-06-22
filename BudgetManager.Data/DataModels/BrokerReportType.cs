using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents the type of a broker report.
    /// </summary>
    public class BrokerReportType : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the broker report type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code representing the broker report type.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the broker report type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of broker reports associated with this type.
        /// </summary>
        public List<BrokerReportToProcess> BrokerReportsToProcess { get; set; }
    }
}
