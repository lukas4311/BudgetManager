using BudgetManager.Core.Extensions;
using BudgetManager.Data;
using BudgetManager.FinanceDataMining.Comodity;
using BudgetManager.FinanceDataMining.CryproApi;
using BudgetManager.FinanceDataMining.Models;
using BudgetManager.InfluxDbData;
using BudgetManager.InfluxDbData.Models;
using Microsoft.EntityFrameworkCore;
using BudgetManager.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BudgetManager.TestingConsole.Crypto;
using System;

namespace BudgetManager.TestingConsole
{
    [Obsolete("All scraping and parsing moved to Python scripts.")]
    public class ProcessManager
    {
        private const string organizationId = "f209a688c8dcfff3";
        private const string bucketCrypto = "Crypto";
        private const string bucketForex = "Forex";
        private const string bucketFearAndGreed = "CryptoFearAndGreed";
        private const string buckerComodity = "Comodity";
        private const string bucketHashRate = "HashRate";
        private const string gold = "AU";
        private readonly ConfigManager configManager;
        private readonly DataContext DataContext;

        public ProcessManager()
        {
            this.configManager = new ConfigManager();
            this.DataContext = this.GetDataContext();
        }

        /// <summary>
        /// Download history values of crypto (from last updated value)
        /// </summary>
        /// <param name="cryptoTicker">Crypto ticker</param>
        /// <returns>Task</returns>
        internal async Task DownloadCryptoHistory(CryptoTicker cryptoTicker)
        {
            InfluxConfig config = GetSecretToken();

            //InfluxDbData.Repository<CryptoData> repo = new(new InfluxContext(config.Url, config.Token));
            //IEnumerable<CryptoData> lastRecords = await repo.GetLastWrittenRecordsTime(new DataSourceIdentification(organizationId, bucketCrypto)).ConfigureAwait(false);
            //CryptoData lastTickerRecord = lastRecords.SingleOrDefault(r => r.Ticker == cryptoTicker.ToString());

            //CryptoDataDownloader dataDownloader = new CryptoDataDownloader(repo, new DataSourceIdentification(organizationId, bucketCrypto));
            //await dataDownloader.CryptoDownload(cryptoTicker, lastTickerRecord?.Time).ConfigureAwait(false);
        }

        internal async Task DownloadForexHistory(ForexTicker forexTicker)
        {
            InfluxConfig config = GetSecretToken();

            //InfluxDbData.Repository<ForexData> repo = new(new InfluxContext(config.Url, config.Token));
            //ForexDataDownloader forexDataDownloader = new ForexDataDownloader(repo, new DataSourceIdentification(organizationId, bucketForex), this.configManager.GetTwelveDataApiKey());
            //await forexDataDownloader.ForexDownload(forexTicker.Value).ConfigureAwait(false);
        }

        /// <summary>
        /// Download all assets to SQL database
        /// </summary>
        internal async Task DownloadAssets()
        {
            //CryptoWatch cryptoWatch = new CryptoWatch(new HttpClient());
            //IEnumerable<CryptoAsset> assets = await cryptoWatch.GetAssets();
            //ICryptoTickerRepository cryptoTickerRepository = new CryptoTickerRepository(this.DataContext);
            //assets = this.FilterNotSavedAssets(assets);

            //foreach (CryptoAsset asset in assets)
            //    this.CreateTickerEntity(asset, cryptoTickerRepository);

            //cryptoTickerRepository.Save();
        }

        /// <summary>
        /// Download data about fear and greed on crypto market and save it to Influx
        /// </summary>
        internal async Task DownloadFearAndGreed()
        {
            InfluxConfig config = configManager.GetSecretToken();
            FearAndGreed fearApi = new FearAndGreed(new HttpClient());
            IEnumerable<FearAndGreedData> data = (await fearApi.GetFearAndGreedFrom(new System.DateTime(2018,3,1))).Data.Select(g => new FearAndGreedData
            {
                Value = double.Parse(g.Value),
                Time = g.Timestamp.ParseToUtcDateTime()
            });
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketFearAndGreed);
            //InfluxDbData.Repository<FearAndGreedData> repo = new InfluxDbData.Repository<FearAndGreedData>(new InfluxContext(config.Url, config.Token));
            //FearAndGreedData lastRecord = (await repo.GetLastWrittenRecordsTime(dataSourceIdentification)).SingleOrDefault();

            //foreach (FearAndGreedData model in data.Where(f => f.Time > (lastRecord?.Time ?? DateTime.MinValue)))
            //    await repo.Write(model, dataSourceIdentification).ConfigureAwait(false);
        }

        /// <summary>
        /// Download data and save them to Influx
        /// </summary>
        internal async Task SaveGoldDataToDb()
        {
            InfluxConfig config = configManager.GetSecretToken();
            GoldApi goldApi = new GoldApi(new HttpClient(), configManager.GetQuandlSetting().ApiKey);
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, buckerComodity);
            //InfluxDbData.Repository<ComodityData> repo = new InfluxDbData.Repository<ComodityData>(new InfluxContext(config.Url, config.Token));
            //ComodityData lastRecord = (await repo.GetLastWrittenRecordsTime(dataSourceIdentification))
            //    .SingleOrDefault(t => string.Compare(t.Ticker, gold, true) == 0);

            //IEnumerable<ComodityData> data = (await goldApi.GetData(lastRecord?.Time ?? DateTime.MinValue).ConfigureAwait(false)).Select(g => new ComodityData
            //{
            //    Price = g.Price,
            //    Ticker = gold,
            //    Time = g.Time.ToUniversalTime()
            //});

            //foreach (ComodityData model in data)
            //    await repo.Write(model, dataSourceIdentification).ConfigureAwait(false);
        }

        internal async Task DownloadHashRate()
        {
            InfluxConfig configSecrets = configManager.GetSecretToken();
            HashRateApi hashRateApi = new HashRateApi(new HttpClient(), configManager.GetQuandlSetting().ApiKey);

            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketHashRate);
            //InfluxDbData.Repository<HashRate> repo = new InfluxDbData.Repository<HashRate>(new InfluxContext(configSecrets.Url, configSecrets.Token));
            //HashRate lastRecord = (await repo.GetLastWrittenRecordsTime(dataSourceIdentification).ConfigureAwait(false)).SingleOrDefault();

            //IEnumerable<HashRate> data = (await hashRateApi.GetData(lastRecord?.Time ?? DateTime.MinValue)).Select(g => new HashRate
            //{
            //    Value = g.Value,
            //    Time = g.Time.ToUniversalTime()
            //});

            //foreach (HashRate model in data)
            //    await repo.Write(model, dataSourceIdentification).ConfigureAwait(false);
        }

        internal void SaveCoinbaseDataToDb()
        {
            //BudgetManager.Repository.IRepository<CryptoTicker> cryptoTickerRepository = new Repository<CryptoTicker>(this.DataContext);
            //BudgetManager.Repository.IRepository<CurrencySymbol> currencySymbolRepository = new CurrencySymbolRepository(this.DataContext);
            //BudgetManager.Repository.IRepository<CryptoTradeHistory> cryptoTradeHistoryRepository = new CryptoTradeHistoryRepository(this.DataContext);
            //CoinbaseParser coinbaseParser = new CoinbaseParser(cryptoTickerRepository, currencySymbolRepository, cryptoTradeHistoryRepository);
            //coinbaseParser.ParseCoinbaseReport();
        }

        internal void ParseCoinbaseReport()
        {
            //var coinbaseParser = new CoinbaseParser(new CryptoTickerRepository(this.GetDataContext()), new CurrencySymbolRepository(this.GetDataContext()), new CryptoTradeHistoryRepository(this.GetDataContext()));
            //coinbaseParser.ParseCoinbaseReport();
        }

        private DataContext GetDataContext()
        {
            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(this.configManager.GetConnectionString());
            return new DataContext(optionsBuilder.Options);
        }

        private IEnumerable<CryptoAsset> FilterNotSavedAssets(IEnumerable<CryptoAsset> allAssets)
        {
            //ICryptoTickerRepository cryptoTickerRepository = new CryptoTickerRepository(this.DataContext);
            //List<Data.DataModels.CryptoTicker> existingAssets = cryptoTickerRepository.FindAll().ToList();
            //return allAssets.Where(a => !existingAssets.Any(ea => string.Compare(ea.Ticker, a.Symbol, true) == 0));
            return null;
        }

        private void CreateTickerEntity(CryptoAsset cryptoAsset, BudgetManager.Repository.IRepository<Data.DataModels.CryptoTicker> cryptoTickerRepository)
        {
            cryptoTickerRepository.Create(new Data.DataModels.CryptoTicker
            {
                Name = cryptoAsset.Name,
                Ticker = cryptoAsset.Symbol
            });
        }

        private InfluxConfig GetSecretToken() => configManager.GetSecretToken();
    }
}
