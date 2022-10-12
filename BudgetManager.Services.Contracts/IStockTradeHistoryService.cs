using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface IStockTradeHistoryService : IBaseService<StockTradeHistoryModel, StockTradeHistory>
    {
        IEnumerable<StockTradeHistoryGetModel> GetAll(int userId);

        public bool UserHasRightToPayment(int stockTradeHistoruId, int userId);
    }
}
