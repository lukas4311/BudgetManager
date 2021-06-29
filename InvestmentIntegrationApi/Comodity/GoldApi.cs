using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BudgetManager.FinanceDataMining.Models.Dtos;
using BudgetManager.FinanceDataMining.Models;
using BudgetManager.FinanceDataMining.Services;
using System.Linq;

namespace BudgetManager.FinanceDataMining.Comodity
{
    public class GoldApi
    {
        private const string goldDataUrl = "https://www.quandl.com/api/v3/datasets/LBMA/GOLD.json?api_key=";
        private readonly QuandlApi QuandlApi;

        public GoldApi(HttpClient httpClient, string apiKey)
        {
            this.QuandlApi = new QuandlApi(httpClient, apiKey);
        }

        public async Task<IEnumerable<GoldModel>> GetData()
            => await this.GetData(DateTime.MinValue);

        public async Task<IEnumerable<GoldModel>> GetData(DateTime fromDate)
        {
            return (await this.QuandlApi.GetData<GoldDataModel>(goldDataUrl, fromDate)).Select(h => new GoldModel
            {
               Time = h.Date,
               Price = h.GoldPrice
            });
        }
    }
}
