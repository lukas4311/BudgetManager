namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a model for user identification.
/// </summary>
public class UserIdentification
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the username associated with the user.
    /// </summary>
    public string UserName { get; set; }
}