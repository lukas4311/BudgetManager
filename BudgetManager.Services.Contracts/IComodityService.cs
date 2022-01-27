using BudgetManager.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    public interface IComodityService
    {
        void Add(ComodityTradeHistoryModel tradeHistory);

        void Delete(int id);

        IEnumerable<ComodityTradeHistoryModel> GetByUser(string userLogin);

        IEnumerable<ComodityTradeHistoryModel> GetByUser(int userId);

        IEnumerable<ComodityTypeModel> GetComodityTypes();

        IEnumerable<ComodityUnitModel> GetComodityUnits();

        Task<double> GetCurrentGoldPriceForOunce();

        void Update(ComodityTradeHistoryModel tradeHistory);

        bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId);
    }
}