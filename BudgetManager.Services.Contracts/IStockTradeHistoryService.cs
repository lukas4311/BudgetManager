using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.InfluxDbData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    public interface IStockTradeHistoryService : IBaseService<StockTradeHistoryModel, StockTradeHistory>
    {
        IEnumerable<StockTradeHistoryGetModel> GetAll(int userId);

        bool UserHasRightToPayment(int stockTradeHistoruId, int userId);

        Task<IEnumerable<StockPrice>> GetStockPriceHistory(string ticker);
    }
}
