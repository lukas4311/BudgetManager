namespace BudgetManager.Domain.MessagingContracts;

/// <summary>
/// Model proevent message of new stock ticker request
/// </summary>
public class TickerRequest
{
    /// <summary>
    /// Stock ticker
    /// </summary>
    public string Ticker { get; set; }

    /// <summary>
    /// User Id
    /// </summary>
    public int UserId { get; set; }
}