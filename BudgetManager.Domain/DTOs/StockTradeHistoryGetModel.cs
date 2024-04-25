namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a model for stock trade history with additional information.
/// Inherits from the base <see cref="StockTradeHistoryModel"/> class.
/// </summary>
public class StockTradeHistoryGetModel : StockTradeHistoryModel
{
    /// <summary>
    /// Gets or sets the currency symbol associated with the trade.
    /// </summary>
    public string CurrencySymbol { get; set; }

    /// <summary>
    /// Gets or sets the trade size after accounting for stock splits.
    /// </summary>
    public double TradeSizeAfterSplit { get; set; }
}