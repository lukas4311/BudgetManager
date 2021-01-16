using FinanceDataMining.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SystemInterface;

namespace FinanceDataMining.CryproApi
{
    public class CryptoCandleDataApi
    {
        private readonly HttpClient httpClient;
        private readonly IDateTime dateTime;
        private const string dateFormat = "yyyy-MM-dd HH:mm:ss";

        public CryptoCandleDataApi(HttpClient httpClient, IDateTime dateTime)
        {
            this.httpClient = httpClient;
            this.dateTime = dateTime;
        }

        public async Task<List<CandleModel>> GetPreviousMonthCryptoCandles(string cryptoSymbol)
        {
            string now = this.dateTime.Now.DateTimeInstance.ToString(dateFormat);
            string prevoiusMonth = this.dateTime.Now.AddMonths(-1).DateTimeInstance.ToString(dateFormat);
            string url = $"https://cryptocandledata.com/api/candles?exchange=coinbase&tradingPair={cryptoSymbol}&interval=1d&startDateTime={prevoiusMonth}&endDateTime={now}";
            var data = await this.httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<List<CandleModel>>(data);
        }
    }
}
