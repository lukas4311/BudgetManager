using BudgetManager.Core.Extensions;
using BudgetManager.FinanceDataMining.Models;
using BudgetManager.FinanceDataMining.CryproApi;
using BudgetManager.FinanceDataMining.Models;
using InfluxDbData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BudgetManager.TestingConsole
{
    internal class CryptoDataDownloader
    {
        private readonly IRepository<CryptoData> influxRepo;
        private readonly DataSourceIdentification dataSourceIdentification;

        public CryptoDataDownloader(IRepository<CryptoData> influxRepository, DataSourceIdentification dataSourceIdentification)
        {
            this.influxRepo = influxRepository;
            this.dataSourceIdentification = dataSourceIdentification;
        }

        public async Task CryptoDownload(CryptoTicker tickerToDownload, DateTime? from = null)
        {
            List<CandleModel> data = await this.DownloadData(tickerToDownload, from ?? new DateTime(2019,1,1)).ConfigureAwait(false);

            foreach (CandleModel model in data)
            {
                await influxRepo.Write(new CryptoData
                {
                    ClosePrice = model.Close,
                    HighPrice = model.High,
                    LowPrice = model.Low,
                    OpenPrice = model.Open,
                    Ticker = tickerToDownload.ToString(),
                    Time = model.DateTime.ParseToUtcDateTime(),
                    Volume = model.Volume
                }, this.dataSourceIdentification).ConfigureAwait(false);
            }
        }

        private async Task<List<CandleModel>> DownloadData(CryptoTicker cryptoTicker, DateTime from)
        {
            CryptoWatch cryptoCandleDataApi = new CryptoWatch(new HttpClient());
            return await cryptoCandleDataApi.GetCandlesDataFrom(cryptoTicker.ToString(), from).ConfigureAwait(false);
        }
    }
}
