namespace BudgetManager.Domain.MessagingContracts;

public class TickerRequest
{
    public string Ticker { get; set; }

    public int UserId { get; set; }
}