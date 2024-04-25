namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a stock ticker entity.
/// </summary>
public class StockTickerModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the stock ticker.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the ticker symbol associated with the stock.
    /// </summary>
    public string Ticker { get; set; }

    /// <summary>
    /// Gets or sets the name of the stock.
    /// </summary>
    public string Name { get; set; }
}