using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMining.StockApi.JsonModelDto;
using System;
using FinanceDataMining.Extensions;

namespace FinanceDataMining.StockApi
{
    public partial class FinnhubStockApi : IFinnhubStockApi
    {
        private readonly HttpClient httpClient;

        public FinnhubStockApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task GetPreviousMonthCandles(string ticker)
        {
            await this.GetStockData(DateTime.Now, DateTime.Now.AddMonths(-1), ticker).ConfigureAwait(false);
        }

        public async Task GetRealTimeQuoteData(string ticker)
        {
            await this.GetStockData(DateTime.Now, DateTime.Now, ticker).ConfigureAwait(false);
        }

        private async Task GetStockData(DateTime from, DateTime to, string ticker)
        {
            string fetchedData = await this.httpClient.GetStringAsync($"https://finnhub.io/api/v1/stock/candle?symbol={ticker}&resolution=1&from={from.ConvertToUnixTimestamp()}&to={to.ConvertToUnixTimestamp()}&token={"test"}").ConfigureAwait(false);

            QuoteData quoteData = JsonConvert.DeserializeObject<QuoteData>(fetchedData);
        }
    }
}
