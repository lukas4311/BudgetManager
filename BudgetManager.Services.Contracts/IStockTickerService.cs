using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Stock ticker service.
    /// </summary>
    public interface IStockTickerService : IBaseService<StockTickerModel, StockTicker, IRepository<StockTicker>>
    {
    }
}
