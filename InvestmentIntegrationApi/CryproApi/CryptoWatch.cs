using FinanceDataMining.Enums;
using FinanceDataMining.Extensions;
using FinanceDataMining.Models;
using Newtonsoft.Json;
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
        private const string cryptoWatchBaseUrl = "https://api.cryptowat.ch/";
        private Exchanges exchange = Exchanges.CoinbasePro;
        private int fourHourCandles = 14400;

        public CryptoWatch(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public void ChangeExchange(Exchanges exchange) => this.exchange = exchange;

        public async Task<List<CandleModel>> GetCandlesDataFrom(string cryptoSymbol, DateTime dateTime)
        {
            long from = ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            string url = $"{cryptoWatchBaseUrl}/markets/{exchange.ToDescriptionString()}/{cryptoSymbol}/ohlc?periods={fourHourCandles}&after={from}";
            string response = await this.httpClient.GetStringAsync(url);
            CandleStickRootModel data = JsonConvert.DeserializeObject<CandleStickRootModel>(response);
            List<CandleModel> candleModels = new List<CandleModel>();

            foreach (var item in data.CandleStickData.FourHour)
                candleModels.Add(this.ParseToCandleModel(item));

            return candleModels;
        }

        public async Task<IEnumerable<CryptoAsset>> GetAssets()
        {
            string url = $"{cryptoWatchBaseUrl}/assets";
            string response = await this.httpClient.GetStringAsync(url);
            CryptoAssetRoot data = JsonConvert.DeserializeObject<CryptoAssetRoot>(response);

            return data.Assets;
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
