using InfluxDbData;
using System.Threading.Tasks;

namespace ManagerWeb.Services
{
    public interface IForexService
    {
        Task<ForexData> GetCurrentExchangeRate(string fromSymbol, string toSymbol);
    }
}