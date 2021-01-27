using FinanceDataMining.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceDataMining.CryproApi
{

    /// <summary>
    /// https://docs.cryptowat.ch/home/
    /// </summary>
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
            List<CandleModel> candleModels = new List<CandleModel>();


            foreach (var item in data.CandleStickData.FourHour)
            {
                candleModels.Add(this.ParseToCandleModel(item));
            }

            return candleModels;
        }

        private CandleModel ParseToCandleModel(List<double> item)
        {
            return new CandleModel
            {
                DateTime = item[0].ToString(),
                Open = item[1],
                High = item[2],
                Low = item[3],
                Close = item[4],
                Volume = item[5]
            };
        }
    }
}
