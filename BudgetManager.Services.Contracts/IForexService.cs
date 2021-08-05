using System.Threading.Tasks;

namespace BudgetManager.Services.Contracts
{
    public interface IForexService
    {
        Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol);
    }
}