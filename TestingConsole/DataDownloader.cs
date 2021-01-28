using FinanceDataMining.CryproApi;
using FinanceDataMining.Models;
using InfluxDbData;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestingConsole
{
    internal class DataDownloader
    {
        const string organizationId = "8f46f33452affe4a";
        const string bucket = "Crypto";

        public async Task CryptoDownload(InfluxConfig config, CryptoTicker tickerToDownload)
        {
            List<CandleModel> data = await this.DownloadData(tickerToDownload, new DateTime(2019, 1, 1)).ConfigureAwait(false);

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

        private async Task<List<CandleModel>> DownloadData(CryptoTicker cryptoTicker, DateTime from)
        {
            CryptoWatch cryptoCandleDataApi = new CryptoWatch(new HttpClient());
            return await cryptoCandleDataApi.GetCandlesDataFrom(cryptoTicker.ToString(), from).ConfigureAwait(false);
        }
    }
}
