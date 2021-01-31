using Data;
using FinanceDataMining.CryproApi;
using FinanceDataMining.Models;
using InfluxDbData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestingConsole
{
    class Program
    {
        const string organizationId = "8f46f33452affe4a";
        const string bucketCrypto = "Crypto";
        const string bucketForex = "Forex";

        static async Task Main(string[] args)
        {

            ConfigManager configManager = new ConfigManager();
            InfluxConfig config = configManager.GetSecretToken();

            //Repository<CryptoData> repo = new Repository<CryptoData>(new InfluxContext(config.Url, config.Token));
            //var data = await repo.GetLastWrittenRecordTime(new DataSourceIdentification(organizationId, bucketCrypto));
            //CryptoDataDownloader dataDownloader = new CryptoDataDownloader(repo, new DataSourceIdentification(organizationId, bucketCrypto));
            //await dataDownloader.CryptoDownload(CryptoTicker.SNXUSD).ConfigureAwait(false);

            //Repository<ForexData> repo = new Repository<ForexData>(new InfluxContext(config.Url, config.Token));
            //ForexDataDownloader forexDataDownloader = new ForexDataDownloader(repo, new DataSourceIdentification(organizationId, bucketForex));
            //await forexDataDownloader.ForexDownload(ForexTicker.USD).ConfigureAwait(false);

            CryptoWatch cryptoWatch = new CryptoWatch(new HttpClient());
            IEnumerable<CryptoAsset> assets = await cryptoWatch.GetAssets();

            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(configManager.GetConnectionString());
            DataContext dataContext = new DataContext(optionsBuilder.Options);   
        }
    }
}
