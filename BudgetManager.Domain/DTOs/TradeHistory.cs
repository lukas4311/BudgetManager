using System;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a model for trade history.
/// Implements the <see cref="IUserDtoModel"/> interface.
/// </summary>
public class TradeHistory : IUserDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the trade history.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the trade.
    /// </summary>
    public DateTime TradeTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the crypto ticker associated with the trade.
    /// </summary>
    public int CryptoTickerId { get; set; }

    /// <summary>
    /// Gets or sets the crypto ticker symbol associated with the trade.
    /// </summary>
    public string CryptoTicker { get; set; }

    /// <summary>
    /// Gets or sets the size of the trade.
    /// </summary>
    public double TradeSize { get; set; }

    /// <summary>
    /// Gets or sets the value of the trade.
    /// </summary>
    public double TradeValue { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the currency symbol associated with the trade.
    /// </summary>
    public int CurrencySymbolId { get; set; }

    /// <summary>
    /// Gets or sets the currency symbol associated with the trade.
    /// </summary>
    public string CurrencySymbol { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user associated with the trade.
    /// </summary>
    public int UserIdentityId { get; set; }
}