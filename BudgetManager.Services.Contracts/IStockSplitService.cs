using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface IStockSplitService : IBaseService<StockSplitModel, StockSplit>
    {
        IEnumerable<StockSplitAccumulated> GetSplitAccumulated();
    }
}
