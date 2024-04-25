namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a commodity type model.
/// </summary>
public class ComodityTypeModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the commodity type (nullable).
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the code associated with the commodity type.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the name of the commodity type.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the associated commodity unit.
    /// </summary>
    public int ComodityUnitId { get; set; }

    /// <summary>
    /// Gets or sets the name of the commodity unit.
    /// </summary>
    public string ComodityUnit { get; set; }
}