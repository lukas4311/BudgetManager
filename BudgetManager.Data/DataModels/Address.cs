namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents an address entity with geographic details.
    /// </summary>
    public class Address : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the country of the address.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the city of the address.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the street name and number of the address.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the state or province of the address.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the ZIP or postal code of the address.
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the address.
        /// </summary>
        public decimal? Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the address.
        /// </summary>
        public decimal? Lng { get; set; }
    }
}
