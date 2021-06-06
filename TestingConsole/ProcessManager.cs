using BudgetManager.Core.Extensions;
using Data;
using BudgetManager.FinanceDataMining.Comodity;
using BudgetManager.FinanceDataMining.CryproApi;
using BudgetManager.FinanceDataMining.Models;
using InfluxDbData;
using InfluxDbData.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestingConsole.Crypto;

namespace TestingConsole
{
    public class ProcessManager
    {
        private const string organizationId = "8f46f33452affe4a";
        private const string bucketCrypto = "Crypto";
        private const string bucketForex = "Forex";
        private const string bucketFearAndGreed = "CryptoFearAndGreed";
        private const string buckerComodity = "Comodity";
        private const string gold = "AU";
        private readonly ConfigManager configManager;

        public ProcessManager()
        {
            this.configManager = new ConfigManager();
        }

        public async Task DownloadCryptoHistory(CryptoTicker cryptoTicker)
        {
            InfluxConfig config = configManager.GetSecretToken();

            InfluxDbData.Repository<CryptoData> repo = new InfluxDbData.Repository<CryptoData>(new InfluxContext(config.Url, config.Token));
            List<CryptoData> lastRecords = await repo.GetLastWrittenRecordsTime(new DataSourceIdentification(organizationId, bucketCrypto)).ConfigureAwait(false);
            CryptoData lastTickerRecord = lastRecords.SingleOrDefault(r => r.Ticker == cryptoTicker.ToString());

            CryptoDataDownloader dataDownloader = new CryptoDataDownloader(repo, new DataSourceIdentification(organizationId, bucketCrypto));
            await dataDownloader.CryptoDownload(cryptoTicker, lastTickerRecord.Time).ConfigureAwait(false);
        }

        public async Task DownloadForexHistory(ForexTicker forexTicker)
        {
            InfluxConfig config = configManager.GetSecretToken();

            InfluxDbData.Repository<ForexData> repo = new InfluxDbData.Repository<ForexData>(new InfluxContext(config.Url, config.Token));
            ForexDataDownloader forexDataDownloader = new ForexDataDownloader(repo, new DataSourceIdentification(organizationId, bucketForex));
            await forexDataDownloader.ForexDownload(forexTicker).ConfigureAwait(false);
        }

        public async Task DownloadAssets()
        {
            CryptoWatch cryptoWatch = new CryptoWatch(new HttpClient());
            IEnumerable<CryptoAsset> assets = await cryptoWatch.GetAssets();

            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(configManager.GetConnectionString());
            DataContext dataContext = new DataContext(optionsBuilder.Options);

            ICryptoTickerRepository cryptoTickerRepository = new CryptoTickerRepository(dataContext);

            foreach (CryptoAsset asset in assets)
            {
                cryptoTickerRepository.Create(new Data.DataModels.CryptoTicker
                {
                    Name = asset.Name,
                    Ticker = asset.Symbol
                });
            }

            cryptoTickerRepository.Save();
        }

        public async Task DownloadFearAndGreed()
        {
            InfluxConfig config = configManager.GetSecretToken();
            FearAndGreed fearApi = new FearAndGreed(new HttpClient());
            IEnumerable<FearAndGreedData> data = (await fearApi.GetFearAndGreedFrom(new System.DateTime(2018,3,1))).Data.Select(g => new FearAndGreedData
            {
                Value = double.Parse(g.Value),
                Time = g.Timestamp.ParseToUtcDateTime()
            });;
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketFearAndGreed);
            InfluxDbData.Repository<FearAndGreedData> repo = new InfluxDbData.Repository<FearAndGreedData>(new InfluxContext(config.Url, config.Token));

            foreach (FearAndGreedData model in data)
                await repo.Write(model, dataSourceIdentification).ConfigureAwait(false);
        }

        public void SaveCoinbaseDataToDb()
        {
            DataContext dataContext = GetDataContext();
            ICryptoTickerRepository cryptoTickerRepository = new CryptoTickerRepository(dataContext);
            ICurrencySymbolRepository currencySymbolRepository = new CurrencySymbolRepository(dataContext);
            ICryptoTradeHistoryRepository cryptoTradeHistoryRepository = new CryptoTradeHistoryRepository(dataContext);
            CoinbaseParser coinbaseParser = new CoinbaseParser(cryptoTickerRepository, currencySymbolRepository, cryptoTradeHistoryRepository);
            coinbaseParser.ParseCoinbaseReport();
        }

        public async Task SaveGoldDataToDb()
        {
            InfluxConfig config = configManager.GetSecretToken();
            GoldApi goldApi = new GoldApi(new HttpClient());
            IEnumerable<ComodityData> data = (await goldApi.GetGoldData().ConfigureAwait(false)).Select(g => new ComodityData
            {
                Price = (double)g.Item2,
                Ticker = gold,
                Time = g.Item1.ToUniversalTime()
            });
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, buckerComodity);
            InfluxDbData.Repository<ComodityData> repo = new InfluxDbData.Repository<ComodityData>(new InfluxContext(config.Url, config.Token));

            foreach (ComodityData model in data)
                await repo.Write(model, dataSourceIdentification).ConfigureAwait(false);
        }

        private DataContext GetDataContext()
        {
            ConfigManager configManager = new ConfigManager();
            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(configManager.GetConnectionString());
            return new DataContext(optionsBuilder.Options);
        }
    }
}
