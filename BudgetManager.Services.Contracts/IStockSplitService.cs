using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface IStockSplitService : IBaseService<StockSplitModel, StockSplit>
    {
        IEnumerable<(int tickerId, List<StockSplitAccumulated> splits)> GetGrupedAccumulatedSplits();
        IEnumerable<StockSplitAccumulated> GetSplitAccumulated();
        IEnumerable<StockSplitAccumulated> GetTickerSplits(int tickerId);
        IEnumerable<List<StockSplitAccumulated>> AccumulateSplits(IEnumerable<List<StockSplitAccumulated>> accumulatedData);
        double GetAccumulatedCoefficient(IEnumerable<List<StockSplitModel>> accumulatedData);
    }
}
