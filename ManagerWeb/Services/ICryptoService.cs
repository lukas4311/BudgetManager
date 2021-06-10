using BudgetManager.InfluxDbData;
using BudgetManager.ManagerWeb.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.ManagerWeb.Services
{
    public interface ICryptoService
    {
        IEnumerable<TradeHistory> Get();

        TradeHistory Get(int id);

        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);
    }
}