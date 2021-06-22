using System;
using System.Collections.Generic;
using System.Net.Http;
using BudgetManager.FinanceDataMining.Models;

namespace BudgetManager.FinanceDataMining.CryproApi
{
    class QuandlApi
    {
        private const string goldDataUrl = "https://www.quandl.com/api/v3/datasets/BCHAIN/HRATE.json?api_key=";
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public QuandlApi(HttpClient httpClient, string apiKey)
        {
            this.httpClient = httpClient;
            this.apiKey = apiKey;
        }

        public IEnumerable<HashRateData> GetHashRateData()
        {
            throw new NotImplementedException();
        }
    }
}
