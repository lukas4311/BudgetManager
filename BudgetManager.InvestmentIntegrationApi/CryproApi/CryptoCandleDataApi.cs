using BudgetManager.FinanceDataMining.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SystemInterface;

namespace BudgetManager.FinanceDataMining.CryproApi
{
    [Obsolete]
    public class CryptoCandleDataApi
    {
        private readonly HttpClient httpClient;
        private const string dateFormat = "yyyy-MM-dd HH:mm:ss";

        public CryptoCandleDataApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<CandleModel>> GetCandlesMonthData(string cryptoSymbol, DateTime dateTime)
        {
            string start = dateTime.ToString(dateFormat);
            string end = dateTime.AddMonths(1).ToString(dateFormat);
            string url = $"https://cryptocandledata.com/api/candles?exchange=coinbase&tradingPair={cryptoSymbol}&interval=1d&startDateTime={start}&endDateTime={end}";
            var data = await this.httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<List<CandleModel>>(data);
        }
    }
}
