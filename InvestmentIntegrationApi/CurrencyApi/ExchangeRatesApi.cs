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
        private readonly HttpClient httpClient;

        public ExchangeRatesApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task GetCurrencyHistory(DateTime from, DateTime to, string currencyCode)
        {
            string json = await httpClient.GetStringAsync($"https://api.exchangeratesapi.io/history?start_at={from.Date:yyyy-MM-dd}&end_at={to.Date:yyyy-MM-dd}&base={currencyCode}").ConfigureAwait(false);
            this.ParseDonwloadedData(json);
        }

        public async Task GetActualValueOfCurrency(string currencyCode)
        {
            await this.GetCurrencyHistory(DateTime.Now, DateTime.Now, currencyCode).ConfigureAwait(false);
        }

        private void ParseAllProperties(dynamic expandoObject)
        {
            IDictionary<string, object> propertyValues = expandoObject.rates;
            List<CurrencyData> allCurrencyHistoryData = new List<CurrencyData>();

            foreach (string property in propertyValues.Keys)
            {
                CurrencyData currencyData = new CurrencyData
                {
                    Date = DateTime.Parse(property)
                };
                IDictionary<string, object> anotherCurrecies = propertyValues[property] as IDictionary<string, object>;

                foreach (string propertyCurrency in anotherCurrecies.Keys)
                {
                    currencyData.PriceOfAnotherCurrencies.Add((propertyCurrency, decimal.Parse(anotherCurrecies[propertyCurrency].ToString())));
                }

                allCurrencyHistoryData.Add(currencyData);
            }
        }

        private void ParseDonwloadedData(string json)
        {
            ExpandoObjectConverter converter = new ExpandoObjectConverter();
            dynamic deserializedData = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);

            this.ParseAllProperties(deserializedData);
        }
    }
}