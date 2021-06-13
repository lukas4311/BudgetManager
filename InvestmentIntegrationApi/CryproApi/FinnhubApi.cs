using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SystemInterface;
using BudgetManager.FinanceDataMining.Extensions;
using BudgetManager.FinanceDataMining.StockApi.JsonModelDto;
using System;

namespace FinanceDataMining.CryproApi
{
    [Obsolete]
    public partial class FinnhubApi
    {
        private readonly HttpClient httpClient;
        private readonly IDateTime dateTime;

        public FinnhubApi(HttpClient httpClient, IDateTime dateTime)
        {
            this.httpClient = httpClient;
            this.dateTime = dateTime;
        }

        public async Task GetPreviousMonthCryptoCandles(string cryptoSymbol)
        {
            double seconds = this.dateTime.Now.DateTimeInstance.ConvertToUnixTimestamp();
            double secondsPrevoiusMonth = this.dateTime.Now.AddMonths(-1).DateTimeInstance.ConvertToUnixTimestamp();
            string res = await this.httpClient.GetStringAsync($"https://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:{cryptoSymbol}&resolution=1&from={secondsPrevoiusMonth}&to={seconds}&token={"test"}").ConfigureAwait(false);
            CandleData candleData = JsonConvert.DeserializeObject<CandleData>(res);
        }
    }
}
