using System;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a historical balance entry for other investments.
/// </summary>
public class OtherInvestmentBalaceHistoryModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the balance history entry.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the date associated with the balance entry.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the balance amount.
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// Gets or sets the invested amount (if applicable).
    /// </summary>
    public decimal? Invested { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the other investment.
    /// </summary>
    public int OtherInvestmentId { get; set; }
}