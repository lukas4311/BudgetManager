namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a tax setting with its type and associated value.
    /// </summary>
    public class TaxSetting : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tax setting.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of tax.
        /// </summary>
        public string TaxType { get; set; }

        /// <summary>
        /// Gets or sets the value of the tax setting.
        /// </summary>
        public decimal Value { get; set; }
    }
}
