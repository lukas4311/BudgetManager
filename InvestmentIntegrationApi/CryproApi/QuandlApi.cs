using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BudgetManager.FinanceDataMining.Models;
using System.Text.Json;

namespace BudgetManager.FinanceDataMining.CryproApi
{
    class QuandlApi
    {
        private const string hashRateDataUrl = "https://www.quandl.com/api/v3/datasets/BCHAIN/HRATE.json?api_key=";
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public QuandlApi(HttpClient httpClient, string apiKey)
        {
            this.httpClient = httpClient;
            this.apiKey = apiKey;
        }

        public async Task<IEnumerable<(DateTime, decimal)>> GetHashRateData(DateTime fromDate)
        {
            HashRateResponse hashRateData = await this.ReqeuestHashRateData(fromDate);

            throw new NotImplementedException();
        }

        private async Task<HashRateResponse> ReqeuestHashRateData(DateTime from)
        {
            string data = await this.httpClient.GetStringAsync($"{hashRateDataUrl}{this.apiKey}&{from:yyyy-MM-dd}");
            return JsonSerializer.Deserialize<HashRateResponse>(data);
        }

    }
}
