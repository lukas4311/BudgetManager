using BudgetManager.FinanceDataMining.CurrencyApi;
using InfluxDbData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestingConsole
{
    public class ForexDataDownloader
    {
        private readonly IRepository<ForexData> influxRepo;
        private readonly DataSourceIdentification dataSourceIdentification;

        public ForexDataDownloader(IRepository<ForexData> influxRepository, DataSourceIdentification dataSourceIdentification)
        {
            this.influxRepo = influxRepository;
            this.dataSourceIdentification = dataSourceIdentification;
        }

        public async Task ForexDownload(ForexTicker currency)
        {
            ExchangeRatesApi exchangeRatesApi = new ExchangeRatesApi(new HttpClient());

            List<CurrencyData> data = await exchangeRatesApi.GetCurrencyHistory(new DateTime(2019, 1, 1), DateTime.Now, currency.ToString()).ConfigureAwait(false);

            foreach (CurrencyData model in data)
                await this.SaveDataToInflux(model).ConfigureAwait(false);
        }

        private async Task SaveDataToInflux(CurrencyData model)
        {
            foreach ((string currency, decimal value) in model.PriceOfAnotherCurrencies)
            {
                await influxRepo.Write(new ForexData
                {
                    Time = model.Date.ToUniversalTime(),
                    BaseCurrency = model.BaseCurrency,
                    Currency = currency,
                    Price = (double)value
                }, dataSourceIdentification);
            }
        }
    }
}
