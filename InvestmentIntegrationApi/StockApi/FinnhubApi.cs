using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMining.StockApi.JsonModelDto;

namespace FinanceDataMining.StockApi
{
    public partial class FinnhubApi
    {
        private const string Token = "bs445d7rh5rbsfggj800";
        private readonly HttpClient httpClient;

        public FinnhubApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task GetPreviousMonthCandles()
        {
            string res = await this.httpClient.GetStringAsync($"https://finnhub.io/api/v1/stock/candle?symbol=AAPL&resolution=1&from=1572651390&to=1572910590&token={Token}").ConfigureAwait(false);
            CandleData candleData = JsonConvert.DeserializeObject<CandleData>(res);
        }

        public async Task GetRealTimeQuoteData()
        {
            string res = await this.httpClient.GetStringAsync($"https://finnhub.io/api/v1/stock/candle?symbol=AAPL&resolution=1&from=1572651390&to=1572910590&token={Token}").ConfigureAwait(false);
            QuoteData quoteData = JsonConvert.DeserializeObject<QuoteData>(res);
        }
    }
}
