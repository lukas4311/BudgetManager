namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a tag associated with an other investment.
/// </summary>
public class OtherInvestmentTagModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the tag.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the other investment.
    /// </summary>
    public int OtherInvestmentId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the tag.
    /// </summary>
    public int TagId { get; set; }
}