using Data;
using FinanceDataMining.CryproApi;
using FinanceDataMining.Models;
using InfluxDbData;
using InfluxDbData.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestingConsole
{
    public class ProcessManager
    {
        private const string organizationId = "8f46f33452affe4a";
        private const string bucketCrypto = "Crypto";
        private const string bucketForex = "Forex";
        private const string bucketFearAndGreed = "CryptoFearAndGreed";
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
            IEnumerable<FearAndGreedData> data = (await fearApi.GetFearAndGreedFrom(new System.DateTime(2019,1,1))).Data.Select(g => new FearAndGreedData
            {
                Value = double.Parse(g.Value),
                Time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(g.Timestamp)).DateTime.ToUniversalTime()
            });
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketFearAndGreed);
            InfluxDbData.Repository<FearAndGreedData> repo = new InfluxDbData.Repository<FearAndGreedData>(new InfluxContext(config.Url, config.Token));

            foreach (FearAndGreedData model in data)
                await repo.Write(model, dataSourceIdentification).ConfigureAwait(false);
        }
    }
}
