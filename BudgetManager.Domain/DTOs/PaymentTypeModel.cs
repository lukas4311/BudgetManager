namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a payment type.
/// </summary>
public class PaymentTypeModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the payment type.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the payment type.
    /// </summary>
    public string Name { get; set; }
}