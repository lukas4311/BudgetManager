using System.Threading.Tasks;

namespace BudgetManager.ManagerWeb.Services
{
    public interface IForexService
    {
        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);
    }
}