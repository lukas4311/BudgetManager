using FinanceDataMining.Models;
using InfluxDbData;
using System;
using System.Collections.Generic;
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

            //Repository<ForexData> repo = new Repository<ForexData>(new InfluxContext(config.Url, config.Token));
            //CryptoDataDownloader dataDownloader = new CryptoDataDownloader(repo, new DataSourceIdentification(organizationId, bucketCrypto));
            //await dataDownloader.CryptoDownload(CryptoTicker.SNXUSD).ConfigureAwait(false);

            Repository<ForexData> repo = new Repository<ForexData>(new InfluxContext(config.Url, config.Token));
            ForexDataDownloader forexDataDownloader = new ForexDataDownloader(repo, new DataSourceIdentification(organizationId, bucketForex));
            await forexDataDownloader.ForexDownload(ForexTicker.USD).ConfigureAwait(false);
        }
    }
}
