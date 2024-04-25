namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents an interface for data transfer objects (DTOs).
/// </summary>
public interface IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the DTO (nullable).
    /// </summary>
    public int? Id { get; set; }
}