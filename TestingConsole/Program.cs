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

            CryptoTicker tickerToDownload = CryptoTicker.ETHUSD;

            List<CandleModel> data = await dataDownloader.DownloadData(tickerToDownload, new DateTime(2019, 1, 1)).ConfigureAwait(false);

            Repository<CryptoData> influxRepo = new Repository<CryptoData>(new InfluxContext(config.Url, config.Token));
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucket);

            foreach (CandleModel model in data)
            {
                await influxRepo.Write(new CryptoData
                {
                    ClosePrice = model.Close,
                    HighPrice = model.High,
                    LowPrice = model.Low,
                    OpenPrice = model.Open,
                    Ticker = tickerToDownload.ToString(),
                    Time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(model.DateTime)).DateTime.ToUniversalTime(),
                    Volume = model.Volume
                }, dataSourceIdentification);
            }
        }


    }
}
