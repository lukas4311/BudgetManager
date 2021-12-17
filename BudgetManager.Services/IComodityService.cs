using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services
{
    public interface IComodityService
    {
        void Add(ComodityTradeHistoryModel tradeHistory);
        void Delete(int id);
        IEnumerable<ComodityTradeHistoryModel> GetByUser(string userLogin);
        IEnumerable<ComodityTypeModel> GetComodityTypes();
        IEnumerable<ComodityUnitModel> GetComodityUnits();
        void Update(ComodityTradeHistoryModel tradeHistory);
        bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId);
    }
}