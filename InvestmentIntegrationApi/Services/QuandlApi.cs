using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using BudgetManager.FinanceDataMining.Models;
using System.Linq;
using System.Reflection;
using BudgetManager.FinanceDataMining.Attributes;

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

        public async Task<IEnumerable<T>> GetData<T>(string quandlUrl) where T : IQuandlData, new()
            => await this.GetData<T>(quandlUrl, DateTime.MinValue);

        public async Task<IEnumerable<T>> GetData<T>(string quandlUrl, DateTime from) where T : IQuandlData, new()
        {
            QuandlDatasetWrapper quandlData = await this.ReqeuestData<T>(quandlUrl, from);
            List<(string column, int index)> indexesWithColumnNames = this.GetColumnNamesWithIndexes<T>(quandlData);
            List<T> models = new List<T>();

            foreach (List<object> dataValues in quandlData.Dataset.Data)
            {
                T model = new T();
                this.ProcessAllColumnValues(indexesWithColumnNames, model, dataValues);
                models.Add(model);
            }

            return models;
        }

        private void ProcessAllColumnValues<T>(List<(string column, int index)> indexesWithColumnNames, T model, List<object> dataValues) where T : IQuandlData, new()
        {
            foreach ((string columnName, int index) in indexesWithColumnNames)
            {
                object value = dataValues[index];

                if (model.PropertySetters.TryGetValue(columnName, out Action<object> propertySetter))
                    propertySetter.Invoke(value);
            }
        }

        private List<(string column, int index)> GetColumnNamesWithIndexes<T>(QuandlDatasetWrapper quandlData) where T : IQuandlData
        {
            IEnumerable<string> columns = this.FindDataColumnNames<T>();
            List<(string column, int index)> indexesWithColumnNames = new(columns.Count());

            for (int i = 0; i < columns.Count(); i++)
                indexesWithColumnNames.Add((columns.ElementAt(i), quandlData.Dataset.ColumnNames.IndexOf(columns.ElementAt(i))));

            return indexesWithColumnNames;
        }

        private async Task<QuandlDatasetWrapper> ReqeuestData<T>(string dataUrl, DateTime from) where T : IQuandlData, new()
        {
            string rawData = await this.httpClient.GetStringAsync($"{dataUrl}?api_key={this.apiKey}&start_date={from.AddDays(1):yyyy-MM-dd}");
            return JsonSerializer.Deserialize<QuandlDatasetWrapper>(rawData);
        }

        private IEnumerable<string> FindDataColumnNames<T>() where T : IQuandlData
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .SelectMany(p => p.GetCustomAttributes(false))
                   .Where(o => o is DataColumnDescriptionAttribute)
                   .Select(a => ((DataColumnDescriptionAttribute)a).Description);
        }
    }
}
