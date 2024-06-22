namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a setting configuration stored as JSON.
    /// </summary>
    public class Setting : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the setting.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code representing the setting.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the JSON string that contains the setting configuration.
        /// </summary>
        public string JsonSetting { get; set; }
    }
}
