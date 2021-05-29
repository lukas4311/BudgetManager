using FinanceDataMining.Comodity;
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
            await processManager.SaveGoldDataToDb();
        }
    }
}
