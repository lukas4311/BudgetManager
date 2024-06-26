using System;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a commodity trade history model.
/// </summary>
public class ComodityTradeHistoryModel : IUserDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the trade history (nullable).
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the trade.
    /// </summary>
    public DateTime TradeTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the commodity type.
    /// </summary>
    public int ComodityTypeId { get; set; }

    /// <summary>
    /// Gets or sets the name of the commodity type.
    /// </summary>
    public string ComodityType { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the commodity unit.
    /// </summary>
    public int ComodityUnitId { get; set; }

    /// <summary>
    /// Gets or sets the name of the commodity unit.
    /// </summary>
    public string ComodityUnit { get; set; }

    /// <summary>
    /// Gets or sets the size of the trade.
    /// </summary>
    public double TradeSize { get; set; }

    /// <summary>
    /// Gets or sets the value of the trade.
    /// </summary>
    public double TradeValue { get; set; }

    /// <summary>
    /// Gets or sets the identifier for the currency symbol.
    /// </summary>
    public int CurrencySymbolId { get; set; }

    /// <summary>
    /// Gets or sets the currency symbol.
    /// </summary>
    public string CurrencySymbol { get; set; }

    /// <summary>
    /// Gets or sets the user identity ID associated with this trade history.
    /// </summary>
    public int UserIdentityId { get; set; }

    /// <summary>
    /// Gets or sets the company name related to the trade.
    /// </summary>
    public string Company { get; set; }
}