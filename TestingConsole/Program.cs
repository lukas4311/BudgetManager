using BudgetManager.FinanceDataMining.CryproApi;
using BudgetManager.Repository;
using BudgetManager.TestingConsole.Crypto;
using System.Threading.Tasks;

namespace BudgetManager.TestingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProcessManager processManager = new ProcessManager();

            // download all assets
            //await processManager.DownloadAssets(); /*-DONE*/

            // download fear and greed
            //await processManager.DownloadFearAndGreed(); /*-DONE*/

            // download gold data
            //await processManager.SaveGoldDataToDb(); /*-DONE*/

            // download hash rate
            //await processManager.DownloadHashRate(); /*-DONE*/

            // download crypto data
            //await processManager.DownloadCryptoHistory(CryptoTicker.LINKUSD);

            //SaveCoinbaseDataToDb();
            //await processManager.DownloadFearAndGreed();
            //await processManager.SaveGoldDataToDb();

            // actual stock fear and greed
            //FearAndGreed fearAndGreed = new FearAndGreed(new System.Net.Http.HttpClient());
            //var data = await fearAndGreed.GetActualFearAndGreed();
            //var dataOld = await fearAndGreed.GetFearAndGreedFrom(new System.DateTime(2021, 1, 1));

            // process coinbase report
            processManager.ParseCoinbaseReport();

        }
    }
}
