using System;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents an other investment entity.
/// </summary>
public class OtherInvestmentModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the other investment.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the other investment.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the code associated with the other investment.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the name of the other investment.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the opening balance amount.
    /// </summary>
    public int OpeningBalance { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user associated with the other investment.
    /// </summary>
    public int UserIdentityId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the currency symbol associated with the other investment.
    /// </summary>
    public int CurrencySymbolId { get; set; }
}