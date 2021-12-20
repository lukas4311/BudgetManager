using System;
using System.Threading.Tasks;

namespace BudgetManager.TestingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProcessManager processManager = new ProcessManager();

            // download all assets
            //Console.WriteLine("Donwloading assets");
            //await processManager.DownloadAssets(); /*-DONE*/
            //Console.WriteLine("Donwloading assets - DONE");


            // download fear and greed
            Console.WriteLine("\nDonwloading fear and greed");
            await processManager.DownloadFearAndGreed(); /*-DONE*/
            Console.WriteLine("Donwloading fear and greed - DONE");

            // download gold data
            Console.WriteLine("\nDonwloading gold");
            await processManager.SaveGoldDataToDb(); /*-DONE*/
            Console.WriteLine("Donwloading gold - DONE");

            // download hash rate
            Console.WriteLine("\nDonwloading hash rate");
            await processManager.DownloadHashRate(); /*-DONE*/
            Console.WriteLine("Donwloading hash rate - DONE");

            // download crypto data
            Console.WriteLine("\nDonwloading crypto");
            await processManager.DownloadCryptoHistory(CryptoTicker.BTCUSD);
            Console.WriteLine("BTC done");
            await processManager.DownloadCryptoHistory(CryptoTicker.ETHUSD);
            Console.WriteLine("ETH done");
            await processManager.DownloadCryptoHistory(CryptoTicker.SNXUSD);
            Console.WriteLine("SNX done");
            await processManager.DownloadCryptoHistory(CryptoTicker.ATOMUSD);
            Console.WriteLine("ATOM done");
            await processManager.DownloadCryptoHistory(CryptoTicker.LINKUSD);
            Console.WriteLine("LINK done");

            // actual stock fear and greed
            //FearAndGreed fearAndGreed = new FearAndGreed(new System.Net.Http.HttpClient());
            //var data = await fearAndGreed.GetActualFearAndGreed();
            //var dataOld = await fearAndGreed.GetFearAndGreedFrom(new System.DateTime(2021, 1, 1));

            // process coinbase report
            //processManager.ParseCoinbaseReport();

            // download forex
            await processManager.DownloadForexHistory(ForexTicker.EurCzk);
            //await processManager.DownloadForexHistory(ForexTicker.CzkEur);
            //await processManager.DownloadForexHistory(ForexTicker.UsdCzk);
            //await processManager.DownloadForexHistory(ForexTicker.CzkUsd);
            //await processManager.DownloadForexHistory(ForexTicker.UsdEur);
            //await processManager.DownloadForexHistory(ForexTicker.EurUsd);
        }
    }
}
