using System.Threading.Tasks;

namespace FinanceDataMining.StockApi
{
    public interface IFinnhubStockApi
    {
        Task GetPreviousMonthCandles(string ticker);

        Task GetRealTimeQuoteData(string ticker);
    }
}