using BudgetManager.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    public interface ICryptoService : IBaseService<TradeHistory>
    {
        IEnumerable<TradeHistory> GetByUser(string userLogin);

        IEnumerable<TradeHistory> GetByUser(int userId);

        TradeHistory Get(int id, int userId);

        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);

        bool UserHasRightToCryptoTrade(int cryptoTradeId, int userId);
    }
}