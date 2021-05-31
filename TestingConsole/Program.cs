using Data;
using FinanceDataMining.CryproApi;
using Microsoft.EntityFrameworkCore;
using Repository;
﻿using FinanceDataMining.Comodity;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProcessManager processManager = new ProcessManager();
            //await processManager.DownloadAssets();
            //await processManager.DownloadCryptoHistory(CryptoTicker.SNXUSD);


            //SaveCoinbaseDataToDb();
            await processManager.DownloadFearAndGreed();

            //FearAndGreed fearAndGreed = new FearAndGreed(new System.Net.Http.HttpClient());
            //var data = await fearAndGreed.GetActualFearAndGreed();
            //var dataOld = await fearAndGreed.GetFearAndGreedFrom(new System.DateTime(2021,1,1));
        }

        //private static void SaveCoinbaseDataToDb()
        //{
        //    DataContext dataContext = GetDataContext();
        //    ICryptoTickerRepository cryptoTickerRepository = new CryptoTickerRepository(dataContext);
        //    ICurrencySymbolRepository currencySymbolRepository = new CurrencySymbolRepository(dataContext);
        //    ICryptoTradeHistoryRepository cryptoTradeHistoryRepository = new CryptoTradeHistoryRepository(dataContext);
        //    CoinbaseParser coinbaseParser = new CoinbaseParser(cryptoTickerRepository, currencySymbolRepository, cryptoTradeHistoryRepository);
        //    coinbaseParser.ParseCoinbaseReport();
        //}

        //private static DataContext GetDataContext()
        //{
        //    ConfigManager configManager = new ConfigManager();
        //    DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        //    optionsBuilder.UseSqlServer(configManager.GetConnectionString());
        //    return new DataContext(optionsBuilder.Options);
        //    await processManager.SaveGoldDataToDb();
        //}
    }
}
