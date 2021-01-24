using FinanceDataMining.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceDataMining.CryproApi
{
    public class CryptoWatch
    {
        private readonly HttpClient httpClient;
        private int fourHourCandles = 14400;

        public CryptoWatch(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<CandleModel>> GetCandlesDataFrom(string cryptoSymbol, DateTime dateTime)
        {
            long from = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            string url = $"https://api.cryptowat.ch/markets/coinbase-pro/{cryptoSymbol}/ohlc?periods={fourHourCandles}&after={from}";
            string response = await this.httpClient.GetStringAsync(url);
            var data = JsonConvert.DeserializeObject<CandleStickRootModel>(response);

            return new List<CandleModel>();
        }
    }
}
