namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a bank balance model.
/// </summary>
public class BankBalanceModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the bank balance.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the opening balance for the account.
    /// </summary>
    public int OpeningBalance { get; set; }

    /// <summary>
    /// Gets or sets the current balance for the account.
    /// </summary>
    public decimal Balance { get; set; }
}