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
        const string bucket = "Crypto";

        static async Task Main(string[] args)
        {

            ConfigManager configManager = new ConfigManager();
            InfluxConfig config = configManager.GetSecretToken();
            DataDownloader dataDownloader = new DataDownloader();

            await dataDownloader.CryptoDownload(config, CryptoTicker.SNXUSD).ConfigureAwait(false);
        }
    }
}
