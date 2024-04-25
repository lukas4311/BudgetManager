using System;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a budget model.
/// </summary>
public class BudgetModel : IUserDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the budget (nullable).
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the start date of the budget.
    /// </summary>
    public DateTime DateFrom { get; set; }

    /// <summary>
    /// Gets or sets the end date of the budget.
    /// </summary>
    public DateTime DateTo { get; set; }

    /// <summary>
    /// Gets or sets the budget amount.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Gets or sets the name of the budget.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the user identity ID associated with this budget.
    /// </summary>
    public int UserIdentityId { get; set; }
}