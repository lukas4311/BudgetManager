namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents the model for adding a tag with associated payment information.
/// </summary>
public class AddTagModel
{
    /// <summary>
    /// Gets or sets the code for the tag.
    /// </summary>
    /// <value>The code representing the tag.</value>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the payment identifier associated with the tag.
    /// </summary>
    /// <value>The ID of the payment.</value>
    public int PaymentId { get; set; }
}