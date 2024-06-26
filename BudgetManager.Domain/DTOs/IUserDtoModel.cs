namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents an interface for user-specific data transfer objects (DTOs).
/// </summary>
public interface IUserDtoModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the user identity ID associated with this DTO.
    /// </summary>
    public int UserIdentityId { get; set; }
}