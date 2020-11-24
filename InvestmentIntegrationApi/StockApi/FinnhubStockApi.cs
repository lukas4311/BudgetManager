using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using FinanceDataMining.StockApi.JsonModelDto;
using System;
using FinanceDataMining.Extensions;
using FinanceDataMining.StockApi.Models;

namespace FinanceDataMining.StockApi
{
    public partial class FinnhubStockApi : IFinnhubStockApi
    {
        private readonly HttpClient httpClient;
        private readonly StockSetting stockSetting;

        public FinnhubStockApi(HttpClient httpClient, StockSetting stockSetting)
        {
            this.httpClient = httpClient;
            this.stockSetting = stockSetting;
        }

        public async Task<StockData> GetPreviousMonthCandles(string ticker)
        {
            return await this.GetStockData(DateTime.Now, DateTime.Now.AddMonths(-1), ticker).ConfigureAwait(false);
        }

        public async Task<StockData> GetRealTimeQuoteData(string ticker)
        {
            return await this.GetStockData(DateTime.Now, DateTime.Now, ticker).ConfigureAwait(false);
        }

        private async Task<StockData> GetStockData(DateTime from, DateTime to, string ticker)
        {
            string fetchedData = await this.httpClient.GetStringAsync($"{this.stockSetting.FinhubApiUrlBase}/candle?symbol={ticker}&resolution=1&from={from.ConvertToUnixTimestamp()}&to={to.ConvertToUnixTimestamp()}&token={stockSetting.Token}").ConfigureAwait(false);
            QuoteData data = JsonConvert.DeserializeObject<QuoteData>(fetchedData);

            return new StockData
            {
                CurrentPrice = data.CurrentPrice,
                Date = DateTimeOffset.FromUnixTimeSeconds(data.Date).Date,
                HighPrice = data.HighPrice,
                LowPrice = data.LowPrice,
                OpenPrice = data.OpenPrice,
                PreviousClosePrice = data.PreviousClosePrice
            };
        }
    }
}
