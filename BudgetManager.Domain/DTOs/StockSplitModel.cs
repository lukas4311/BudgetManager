using System;
using System.Collections.Generic;

namespace BudgetManager.Domain.DTOs;

/// <summary>
/// Represents a stock split event.
/// </summary>
public class StockSplitModel : IDtoModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the stock split.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated stock ticker.
    /// </summary>
    public int StockTickerId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the stock split.
    /// </summary>
    public DateTime SplitTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets additional information about the stock split.
    /// </summary>
    public string SplitTextInfo { get; set; }

    /// <summary>
    /// Gets or sets the coefficient by which the stock split occurred.
    /// </summary>
    public double SplitCoefficient { get; set; }
}


/// <summary>
/// Represents accumulated stock split data.
/// </summary>
public class StockSplitAccumulated
{
    /// <summary>
    /// Gets or sets the unique identifier for the accumulated stock split.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated stock ticker.
    /// </summary>
    public int StockTickerId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the accumulated stock split.
    /// </summary>
    public DateTime SpliDateTime { get; set; }

    /// <summary>
    /// Gets or sets the accumulated coefficient for the stock split.
    /// </summary>
    public double SplitAccumulatedCoeficient { get; set; }
}


/// <summary>
/// Represents a grouping of accumulated stock split data.
/// </summary>
public record GroupedStockAccumulatedSpits(int TickerId, IEnumerable<StockSplitAccumulated> Splits);