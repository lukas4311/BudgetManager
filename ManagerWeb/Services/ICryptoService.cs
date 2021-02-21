using InfluxDbData;
using ManagerWeb.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerWeb.Services
{
    public interface ICryptoService
    {
        IEnumerable<TradeHistory> Get();

        TradeHistory Get(int id);

        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);
    }
}