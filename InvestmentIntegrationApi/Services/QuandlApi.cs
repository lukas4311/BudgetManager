using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using BudgetManager.FinanceDataMining.Models;

namespace BudgetManager.FinanceDataMining.Services
{
    internal class QuandlApi
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public QuandlApi(HttpClient httpClient, string apiKey)
        {
            this.httpClient = httpClient;
            this.apiKey = apiKey;
        }

        public async Task<IEnumerable<T>> GetData<T>(string quandlUrl, DateTime from)
        {
            throw new NotImplementedException();
        }

        private async Task<QuandlDataseteWrapper> ReqeuestData(string dataUrl, DateTime from)
        {
            string data = await this.httpClient.GetStringAsync($"{dataUrl}?api_key={this.apiKey}&{from:yyyy-MM-dd}");
            return JsonSerializer.Deserialize<QuandlDataseteWrapper>(data);
        }

        //public async Task<IEnumerable<(DateTime, decimal)>> GetGoldData()
        //    => await this.GetGoldData(DateTime.MinValue);

        //public async Task<IEnumerable<(DateTime, decimal)>> GetGoldData(DateTime fromDate)
        //{
        //    DataseteWrapper goldPriceData = await this.ReqeuestGoldData(fromDate);
        //    List<(DateTime, decimal)> goldData = new List<(DateTime, decimal)>();
        //    (int dateIndex, int usdIndex) = this.FindDataIndexes(goldPriceData.dataset);

        //    foreach (List<object> dataValues in goldPriceData.dataset.data)
        //        this.ProcessData(dateIndex, usdIndex, dataValues, goldData);

        //    return goldData;
        //}

        //private void ProcessData(int dateIndex, int usdIndex, List<object> dataValues, List<(DateTime, decimal)> goldData)
        //{
        //    DateTime date = DateTime.Parse(dataValues[dateIndex].ToString());
        //    decimal.TryParse(dataValues[usdIndex]?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal usdPrice);

        //    if (date != default && usdPrice != default)
        //        goldData.Add((date, usdPrice));
        //}

        //private async Task<DataseteWrapper> ReqeuestGoldData(DateTime from)
        //{
        //    string data = await this.httpClient.GetStringAsync($"{goldDataUrl}{this.apiKey}&{from:yyyy-MM-dd}");
        //        return JsonSerializer.Deserialize<DataseteWrapper>(data);
        //}

        //private (int dateIndex, int usdIndex) FindDataIndexes(Dataset dataset) =>
        //    (dataset.column_names.IndexOf(dateColumn), dataset.column_names.IndexOf(usdColumn));
    }
}
