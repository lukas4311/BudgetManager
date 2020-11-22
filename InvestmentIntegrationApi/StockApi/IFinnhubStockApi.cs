using FinanceDataMining.StockApi.Models;
using System.Threading.Tasks;

namespace FinanceDataMining.StockApi
{
    public interface IFinnhubStockApi
    {
        Task<StockData> GetPreviousMonthCandles(string ticker);

        Task<StockData> GetRealTimeQuoteData(string ticker);
    }
}