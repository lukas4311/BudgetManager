namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a model for tags.
/// Implements the <see cref="IDtoModel"/> interface.
/// </summary>
public class TagModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the tag.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the code associated with the tag.
    /// </summary>
    public string Code { get; set; }
}