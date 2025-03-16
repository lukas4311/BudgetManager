namespace BudgetManager.ManagerWeb.Models.SettingModels
{
    /// <summary>
    /// Represents the InfluxDB settings.
    /// </summary>
    public class Influxdb
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        public string OrganizationId { get; set; }
    }
}
