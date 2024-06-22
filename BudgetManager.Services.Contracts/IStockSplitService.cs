using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service class for managing operations related to stock splits.
    /// </summary>
    public interface IStockSplitService : IBaseService<StockSplitModel, StockSplit>
    {
        /// <summary>
        /// Retrieves grouped accumulated stock split data.
        /// </summary>
        /// <returns>An enumerable of tuples containing ticker ID and its accumulated stock split data.</returns>
        IEnumerable<(int tickerId, List<StockSplitAccumulated> splits)> GetGrupedAccumulatedSplits();

        /// <summary>
        /// Retrieves accumulated stock split data.
        /// </summary>
        /// <returns>An enumerable of accumulated stock split data.</returns>
        IEnumerable<StockSplitAccumulated> GetSplitAccumulated();

        /// <summary>
        /// Retrieves stock split data for a specific ticker.
        /// </summary>
        /// <param name="tickerId">The ID of the ticker.</param>
        /// <returns>An enumerable of accumulated stock split data for the ticker.</returns>
        IEnumerable<StockSplitAccumulated> GetTickerSplits(int tickerId);

        /// <summary>
        /// Accumulates stock splits coefficient values.
        /// </summary>
        /// <param name="accumulatedData">The accumulated data to process.</param>
        /// <returns>The accumulated data with updated coefficients.</returns>
        IEnumerable<List<StockSplitAccumulated>> AccumulateSplits(IEnumerable<List<StockSplitAccumulated>> accumulatedData);

        /// <summary>
        /// Calculates the accumulated coefficient for a sequence of stock split models.
        /// </summary>
        /// <param name="accumulatedData">The sequence of stock split models.</param>
        /// <returns>The accumulated coefficient.</returns>
        double GetAccumulatedCoefficient(IEnumerable<StockSplitModel> accumulatedData);
    }
}
