using BudgetManager.FinanceDataMining.CryproApi;
using System.Threading.Tasks;

namespace BudgetManager.TestingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProcessManager processManager = new ProcessManager();

            // download all assets
            await processManager.DownloadAssets();

            //await processManager.DownloadCryptoHistory(CryptoTicker.SNXUSD);


            //SaveCoinbaseDataToDb();
            //await processManager.DownloadFearAndGreed();
            //await processManager.SaveGoldDataToDb();

            FearAndGreed fearAndGreed = new FearAndGreed(new System.Net.Http.HttpClient());
            var data = await fearAndGreed.GetActualFearAndGreed();
            //var dataOld = await fearAndGreed.GetFearAndGreedFrom(new System.DateTime(2021,1,1));
        }
    }
}
