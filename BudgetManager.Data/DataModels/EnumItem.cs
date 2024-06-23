namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Enumaration item
    /// </summary>
    public class EnumItem : IDataModel
    {
        /// <summary>
        /// Gets or sets the ID of enum item.
        /// </summary>
        public int Id { get ; set; }

        /// <summary>
        /// Gets or sets the code representing enum item.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item type ID
        /// </summary>
        public int EnumItemTypeId { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item type
        /// </summary>
        public EnumItemType EnumItemType { get; set; }
    }
}
