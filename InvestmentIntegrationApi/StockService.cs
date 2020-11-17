using Data;
using FinanceDataMining.CryproApi;
using System.Net.Http;
using SystemWrapper;

namespace FinanceDataMining
{
    public class StockService
    {
        private readonly FinnhubApi finnhubApi;

        public StockService()
        {
            this.finnhubApi = new FinnhubApi(new HttpClient(), new DateTimeWrap());
        }
    }
}
