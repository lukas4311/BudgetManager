namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a bank payment summary model.
/// </summary>
public class BankPaymentSumModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the bank account associated with this payment summary.
    /// </summary>
    public int BankAccountId { get; set; }

    /// <summary>
    /// Gets or sets the total sum of payments for the associated bank account.
    /// </summary>
    public decimal Sum { get; set; }
}