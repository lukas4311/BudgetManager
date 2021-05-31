using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceDataMining.CurrencyApi
{
    public class ExchangeRatesApi
    {
        private const string ExchangeApiUrlBase = "https://api.exchangeratesapi.io";
        private readonly HttpClient httpClient;

        public ExchangeRatesApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<CurrencyData>> GetCurrencyHistory(DateTime from, DateTime to, string currencyCode)
        {
            string json = await httpClient.GetStringAsync($"{ExchangeApiUrlBase}/history?start_at={from.Date:yyyy-MM-dd}&end_at={to.Date:yyyy-MM-dd}&base={currencyCode}").ConfigureAwait(false);

            return this.ParseDonwloadedData(json, currencyCode);
        }

        public async Task GetActualValueOfCurrency(string currencyCode)
        {
            await this.GetCurrencyHistory(DateTime.Now, DateTime.Now, currencyCode).ConfigureAwait(false);
        }

        private List<CurrencyData> ParseAllProperties(dynamic expandoObject, string currencyCode)
        {
            IDictionary<string, object> propertyValues = expandoObject.rates;
            List<CurrencyData> allCurrencyHistoryData = new List<CurrencyData>();

            foreach (string property in propertyValues.Keys)
            {
                CurrencyData currencyData = new CurrencyData
                {
                    Date = DateTime.Parse(property),
                    BaseCurrency = currencyCode
                };

                IDictionary<string, object> anotherCurrecies = propertyValues[property] as IDictionary<string, object>;

                foreach (string propertyCurrency in anotherCurrecies.Keys)
                    currencyData.PriceOfAnotherCurrencies.Add((propertyCurrency, decimal.Parse(anotherCurrecies[propertyCurrency].ToString())));

                allCurrencyHistoryData.Add(currencyData);
            }

            return allCurrencyHistoryData;
        }

        private List<CurrencyData> ParseDonwloadedData(string json, string currencyCode)
        {
            ExpandoObjectConverter converter = new ExpandoObjectConverter();
            dynamic deserializedData = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);

            return this.ParseAllProperties(deserializedData, currencyCode);
        }
    }
}