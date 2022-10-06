using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Services.Contracts
{
    public interface IStockTickerService : IBaseService<StockTickerModel, StockTicker>
    {
    }
}
