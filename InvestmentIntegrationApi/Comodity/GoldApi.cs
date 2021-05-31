using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceDataMining.Comodity.JsonModelDto;

namespace FinanceDataMining.Comodity
{
    public class GoldApi
    {
        private const string goldDataUrl = "https://www.quandl.com/api/v3/datasets/LBMA/GOLD.json";
        private const string dateColumn = "Date";
        private const string usdColumn = "USD (AM)";
        private readonly HttpClient httpClient;

        public GoldApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<(DateTime, decimal)>> GetGoldData()
            => await this.GetGoldData(DateTime.MinValue);

        public async Task<IEnumerable<(DateTime, decimal)>> GetGoldData(DateTime fromDate)
        {
            DataseteWrapper goldPriceData = await this.ReqeuestGoldData(fromDate);
            List<(DateTime, decimal)> goldData = new List<(DateTime, decimal)>();
            (int dateIndex, int usdIndex) = this.FindDataIndexes(goldPriceData.dataset);

            foreach (List<object> dataValues in goldPriceData.dataset.data)
                this.ProcessData(dateIndex, usdIndex, dataValues, goldData);

            return goldData;
        }

        private void ProcessData(int dateIndex, int usdIndex, List<object> dataValues, List<(DateTime, decimal)> goldData)
        {
            DateTime date = DateTime.Parse(dataValues[dateIndex].ToString());
            decimal.TryParse(dataValues[usdIndex]?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal usdPrice);

            if (date != default && usdPrice != default)
                goldData.Add((date, usdPrice));
        }

        private async Task<DataseteWrapper> ReqeuestGoldData(DateTime from)
        {
            string data = await this.httpClient.GetStringAsync($"{goldDataUrl}?{from:yyyy-MM-dd}");
            return JsonSerializer.Deserialize<DataseteWrapper>(data);
        }

        private (int dateIndex, int usdIndex) FindDataIndexes(Dataset dataset) =>
            (dataset.column_names.IndexOf(dateColumn), dataset.column_names.IndexOf(usdColumn));
    }
}
