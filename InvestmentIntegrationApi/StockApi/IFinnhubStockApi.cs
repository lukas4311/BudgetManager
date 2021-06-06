using BudgetManager.FinanceDataMining.StockApi.Models;
using System.Threading.Tasks;

namespace BudgetManager.FinanceDataMining.StockApi
{
    public interface IFinnhubStockApi
    {
        Task<StockData> GetPreviousMonthCandles(string ticker);

        Task<StockData> GetRealTimeQuoteData(string ticker);
    }
}