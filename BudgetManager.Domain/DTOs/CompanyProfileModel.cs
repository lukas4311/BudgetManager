namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a company profile model.
/// </summary>
public class CompanyProfileModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the company profile (nullable).
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the stock symbol associated with the company.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// Gets or sets the name of the company.
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
    /// Gets or sets the industry or sector to which the company belongs.
    /// </summary>
    public string Industry { get; set; }

    /// <summary>
    /// Gets or sets the website URL of the company.
    /// </summary>
    public string Website { get; set; }

    /// <summary>
    /// Gets or sets a brief description of the company.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the sector in which the company operates.
    /// </summary>
    public string Sector { get; set; }
}