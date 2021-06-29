using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BudgetManager.FinanceDataMining.Models;
using System.Text.Json;
using BudgetManager.FinanceDataMining.Services;
using BudgetManager.FinanceDataMining.Models.Dtos;
using System.Linq;

namespace BudgetManager.FinanceDataMining.CryproApi
{
    public class HashRateApi
    {
        private const string hashRateDataUrl = "https://www.quandl.com/api/v3/datasets/BCHAIN/HRATE.json";
        private readonly QuandlApi QuandlApi;

        public HashRateApi(HttpClient httpClient, string apiKey)
        {
            this.QuandlApi = new QuandlApi(httpClient, apiKey);
        }

        public async Task<IEnumerable<HashRateModel>> GetData(DateTime fromDate)
        {
            return (await this.QuandlApi.GetData<HashRateDataModel>(hashRateDataUrl, fromDate)).Select(h => new HashRateModel
            {
                Time = h.Date,
                Value = h.HashRate
            });
        }
    }
}
