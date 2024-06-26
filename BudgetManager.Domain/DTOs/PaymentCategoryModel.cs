namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a payment category.
/// </summary>
public class PaymentCategoryModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the payment category.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the payment category.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the icon associated with the payment category.
    /// </summary>
    public string Icon { get; set; }
}