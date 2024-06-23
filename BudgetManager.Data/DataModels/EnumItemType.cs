namespace BudgetManager.Data.DataModels
{
    public class EnumItemType : IDataModel
    {
        /// <summary>
        /// Gets or sets the ID of enum item type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code representing enum item type.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of enum item type.
        /// </summary>
        public string Name { get; set; }
    }
}
