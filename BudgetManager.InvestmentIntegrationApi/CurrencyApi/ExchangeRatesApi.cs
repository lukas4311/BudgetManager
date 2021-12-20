using BudgetManager.FinanceDataMining.CurrencyApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BudgetManager.FinanceDataMining.CurrencyApi
{
    public class ExchangeRatesApi
    {
        private const string ExchangeApiUrlBase = "https://api.twelvedata.com";
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public ExchangeRatesApi(HttpClient httpClient, string apiKey)
        {
            this.httpClient = httpClient;
            this.apiKey = apiKey;
        }

        public async Task<List<CurrencyData>> GetCurrencyHistory(DateTime from, DateTime to, string currencyCode)
        {
            string json = await httpClient.GetStringAsync($"{ExchangeApiUrlBase}/time_series?apikey={this.apiKey}&start_date={from.Date:yyyy-MM-dd}&end_date={to.Date:yyyy-MM-dd}&symbol={currencyCode}&interval=4h").ConfigureAwait(false);

            return this.ParseDonwloadedData(json, currencyCode);
        }

        public async Task GetActualValueOfCurrency(string currencyCode)
        {
            await this.GetCurrencyHistory(DateTime.Now, DateTime.Now, currencyCode).ConfigureAwait(false);
        }

        //private List<CurrencyData> ParseAllProperties(dynamic expandoObject, string currencyCode)
        //{
        //    IDictionary<string, object> propertyValues = expandoObject.rates;
        //    List<CurrencyData> allCurrencyHistoryData = new List<CurrencyData>();

        //    foreach (string property in propertyValues.Keys)
        //    {
        //        CurrencyData currencyData = new CurrencyData
        //        {
        //            Date = DateTime.Parse(property),
        //            BaseCurrency = currencyCode
        //        };

        //        IDictionary<string, object> anotherCurrecies = propertyValues[property] as IDictionary<string, object>;

        //        foreach (string propertyCurrency in anotherCurrecies.Keys)
        //            currencyData.PriceOfAnotherCurrencies.Add((propertyCurrency, decimal.Parse(anotherCurrecies[propertyCurrency].ToString())));

        //        allCurrencyHistoryData.Add(currencyData);
        //    }

        //    return allCurrencyHistoryData;
        //}

        private List<CurrencyData> ParseDonwloadedData(string json, string currencyCode)
        {
            ExpandoObjectConverter converter = new ExpandoObjectConverter();
            //dynamic deserializedData = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            var data = JsonConvert.DeserializeObject<CurrencyExchangeData>(json);
            int dashLocation = data.Meta.Symbol.IndexOf('/');
            string srcSymbol = data.Meta.Symbol[..(dashLocation + 1)];
            string targetSymbol = data.Meta.Symbol[dashLocation..];

            CurrencyData currencyData = new CurrencyData();
            currencyData.BaseCurrency = data.Meta.Symbol.Substring(0, data.Meta.Symbol.IndexOf("/"));
            //currencyData.Date = 
            return null;
            //return this.ParseAllProperties(deserializedData, currencyCode);
        }
    }

    public class CurrencyInfo
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("currency_base")]
        public string CurrencyBase { get; set; }

        [JsonPropertyName("currency_quote")]
        public string CurrencyQuote { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class PriceData
    {
        [JsonPropertyName("datetime")]
        public string Datetime { get; set; }

        [JsonPropertyName("open")]
        public string Open { get; set; }

        [JsonPropertyName("high")]
        public string High { get; set; }

        [JsonPropertyName("low")]
        public string Low { get; set; }

        [JsonPropertyName("close")]
        public string Close { get; set; }
    }

    public class CurrencyExchangeData
    {
        [JsonPropertyName("meta")]
        public CurrencyInfo Meta { get; set; }

        [JsonPropertyName("values")]
        public List<PriceData> Values { get; set; }
    }
}