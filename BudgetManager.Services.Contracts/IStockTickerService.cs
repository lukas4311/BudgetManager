using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface IStockTickerService : IBaseService<StockTickerModel, StockTicker>
    {
    }
}
