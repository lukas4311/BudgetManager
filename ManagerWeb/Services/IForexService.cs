using System.Threading.Tasks;

namespace ManagerWeb.Services
{
    public interface IForexService
    {
        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);
    }
}