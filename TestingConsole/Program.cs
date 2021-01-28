using FinanceDataMining.Models;
using InfluxDbData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {

            ConfigManager configManager = new ConfigManager();
            InfluxConfig config = configManager.GetSecretToken();
            DataDownloader dataDownloader = new DataDownloader();

            await dataDownloader.CryptoDownload(config, CryptoTicker.SNXUSD).ConfigureAwait(false);
        }
    }
}
