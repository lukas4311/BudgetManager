using FinanceDataMining;
using FinanceDataMining.CryproApi;
using FinanceDataMining.Models;
using InfluxDbData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using SystemInterface;
using SystemWrapper;

namespace TestingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {

            ConfigManager configManager = new ConfigManager();
            InfluxConfig config = configManager.GetSecretToken();

            CryptoWatch cryptoCandleDataApi = new CryptoWatch(new HttpClient());
            List<CandleModel> data = await cryptoCandleDataApi.GetCandlesDataFrom("BTCUSD", new System.DateTime(2019, 1, 1));
            Repository<CryptoData> influxRepo = new Repository<CryptoData>(new InfluxContext(config.Url, config.Token));

            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification("8f46f33452affe4a", "Crypto");

            foreach (CandleModel model in data)
            {
                await influxRepo.Write(new CryptoData
                {
                    ClosePrice = model.Close,
                    HighPrice = model.High,
                    LowPrice = model.Low,
                    OpenPrice = model.Open,
                    Ticker = "BTCUSD",
                    Time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(model.DateTime)).DateTime.ToUniversalTime(),
                    Volume = model.Volume
                }
                , dataSourceIdentification);
            }
        }
    }
}
