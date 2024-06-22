namespace BudgetManager.Data.DataModels
{
    /// <summary>
    /// Represents a company profile.
    /// </summary>
    public class CompanyProfile : IDataModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the company profile.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the symbol representing the company.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the official name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the currency used by the company.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the International Securities Identification Number (ISIN) for the company.
        /// </summary>
        public string Isin { get; set; }

        /// <summary>
        /// Gets or sets the short name of the stock exchange where the company is listed.
        /// </summary>
        public string ExchangeShortName { get; set; }

        /// <summary>
        /// Gets or sets the industry to which the company belongs.
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// Gets or sets the website URL of the company.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets the description of the company.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the sector to which the company belongs.
        /// </summary>
        public string Sector { get; set; }

        /// <summary>
        /// Gets or sets the URL of the company's image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the ID of the address associated with the company.
        /// </summary>
        public int? AddressId { get; set; }

        /// <summary>
        /// Gets or sets the address object associated with the company.
        /// </summary>
        public Address Address { get; set; }
    }
}
