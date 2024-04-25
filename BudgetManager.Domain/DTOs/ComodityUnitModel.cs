namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a commodity unit model.
/// </summary>
public class ComodityUnitModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the commodity unit (nullable).
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the code associated with the commodity unit.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the name of the commodity unit.
    /// </summary>
    public string Name { get; set; }
}