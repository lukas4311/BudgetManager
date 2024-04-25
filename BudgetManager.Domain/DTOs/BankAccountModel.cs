namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents the data transfer object for a bank account.
/// </summary>
public class BankAccountModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the identifier for the bank account.
    /// </summary>
    /// <value>The bank account's ID. Nullable if not yet assigned.</value>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the code associated with the bank account.
    /// </summary>
    /// <value>The unique code representing the bank account.</value>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the opening balance of the bank account.
    /// </summary>
    /// <value>The initial amount of money in the bank account.</value>
    public int OpeningBalance { get; set; }

    /// <summary>
    /// Gets or sets the user identity identifier associated with the bank account.
    /// </summary>
    /// <value>The ID of the user identity linked to the bank account.</value>
    public int UserIdentityId { get; set; }
}