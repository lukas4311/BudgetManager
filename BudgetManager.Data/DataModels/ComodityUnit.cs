using System.Collections.Generic;

namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a unit of measurement for commodities.
    /// </summary>
    public class ComodityUnit : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the commodity unit.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code representing the commodity unit.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the commodity unit.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of commodity types associated with this commodity unit.
        /// </summary>
        public List<ComodityType> ComodityTypes { get; set; }
    }
}
