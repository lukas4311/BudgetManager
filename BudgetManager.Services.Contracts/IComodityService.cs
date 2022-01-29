using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    public interface IComodityService : IBaseService<ComodityTradeHistoryModel>
    {
        IEnumerable<ComodityTradeHistoryModel> GetByUser(string userLogin);

        IEnumerable<ComodityTradeHistoryModel> GetByUser(int userId);

        IEnumerable<ComodityTypeModel> GetComodityTypes();

        IEnumerable<ComodityUnitModel> GetComodityUnits();

        Task<double> GetCurrentGoldPriceForOunce();

        bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId);
    }
}