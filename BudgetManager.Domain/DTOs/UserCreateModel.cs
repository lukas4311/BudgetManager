using System.ComponentModel.DataAnnotations;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a model for creating a user.
/// </summary>
public class UserCreateModel
{
    /// <summary>
    /// Gets or sets the login name for the user (required).
    /// </summary>
    [Required]
    public string Login { get; set; }

    /// <summary>
    /// Gets or sets the password for the user (required).
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the first name of the user (required).
    /// </summary>
    [Required]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user (required).
    /// </summary>
    [Required]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user (required).
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the user (required).
    /// </summary>
    [Required]
    public string Phone { get; set; }
}