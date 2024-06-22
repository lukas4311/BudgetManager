using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a commodity type entity.
    /// </summary>
    public class ComodityType : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the commodity type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code representing the commodity type.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the commodity type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement for the commodity type.
        /// </summary>
        public ComodityUnit ComodityUnit { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the unit of measurement for the commodity type.
        /// </summary>
        public int ComodityUnitId { get; set; }

        /// <summary>
        /// Gets or sets the collection of commodity trade history records associated with this commodity type.
        /// </summary>
        public List<ComodityTradeHistory> ComodityTradeHistory { get; set; }
    }
}
