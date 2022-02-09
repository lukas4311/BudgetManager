using BudgetManager.FinanceDataMining.CurrencyApi;
using BudgetManager.InfluxDbData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BudgetManager.TestingConsole
{
    public class ForexDataDownloader
    {
        private readonly IRepository<ForexData> influxRepo;
        private readonly DataSourceIdentification dataSourceIdentification;
        private readonly string twelveDataapiKey;

        public ForexDataDownloader(IRepository<ForexData> influxRepository, DataSourceIdentification dataSourceIdentification, string twelveDataapiKey)
        {
            this.influxRepo = influxRepository;
            this.dataSourceIdentification = dataSourceIdentification;
            this.twelveDataapiKey = twelveDataapiKey;
        }

        public async Task ForexDownload(string currency)
        {
            ExchangeRatesApi exchangeRatesApi = new ExchangeRatesApi(new HttpClient(), this.twelveDataapiKey);
            DateTime? lastRecordTime = (await this.influxRepo.GetLastWrittenRecordsTime(this.dataSourceIdentification))
                .SingleOrDefault(r => string.Compare(r.Currency, currency.ToString(), true) == 0)?.Time;

            CurrencyData data = await exchangeRatesApi.GetCurrencyHistory(new DateTime(2019, 1, 1), DateTime.Now, currency.ToString()).ConfigureAwait(false);

            foreach ((string currencySymbol, decimal value, DateTime Date) model in data.PriceOfAnotherCurrencies.Where(p => p.Date > (lastRecordTime ?? DateTime.MinValue)))
                await this.SaveDataToInflux(model, data.BaseCurrency).ConfigureAwait(false);
        }

        private async Task SaveDataToInflux((string currencySymbol, decimal value, DateTime Date) model, string baseSymbol)
        {
                await influxRepo.Write(new ForexData
                {
                    Time = model.Date.ToUniversalTime(),
                    BaseCurrency = baseSymbol,
                    Currency = model.currencySymbol,
                    Price = (double)model.value
                }, dataSourceIdentification);
        }
    }
}
