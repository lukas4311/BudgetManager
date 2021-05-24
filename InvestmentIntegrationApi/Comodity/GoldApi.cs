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

        public async Task GetGoldData()
        {
            string data = await this.httpClient.GetStringAsync(goldDataUrl);
            DataseteWrapper goldPriceData = JsonSerializer.Deserialize<DataseteWrapper>(data);
            (int dateIndex, int usdIndex) = this.FindDataIndexes(goldPriceData.dataset);

            foreach (List<object> dataValues in goldPriceData.dataset.data)
            {
                var date = DateTime.Parse(dataValues[dateIndex].ToString());
                var usdPrice = decimal.Parse(dataValues[usdIndex].ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            }
        }

        private (int dateIndex, int usdIndex) FindDataIndexes(Dataset dataset)
        {
            return (dataset.column_names.IndexOf(dateColumn), dataset.column_names.IndexOf(usdColumn));
        }
    }
}
