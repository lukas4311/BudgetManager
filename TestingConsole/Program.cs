using Data;
using FinanceDataMining.Comodity;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Net.Http;
using System.Threading.Tasks;
using TestingConsole.Crypto;

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

            GoldApi goldApi = new GoldApi(new HttpClient());
            await goldApi.GetGoldData();
        }

        private static void SaveCoinbaseDataToDb()
        {
            DataContext dataContext = GetDataContext();
            ICryptoTickerRepository cryptoTickerRepository = new CryptoTickerRepository(dataContext);
            ICurrencySymbolRepository currencySymbolRepository = new CurrencySymbolRepository(dataContext);
            ICryptoTradeHistoryRepository cryptoTradeHistoryRepository = new CryptoTradeHistoryRepository(dataContext);
            CoinbaseParser coinbaseParser = new CoinbaseParser(cryptoTickerRepository, currencySymbolRepository, cryptoTradeHistoryRepository);
            coinbaseParser.ParseCoinbaseReport();
        }

        private static DataContext GetDataContext()
        {
            ConfigManager configManager = new ConfigManager();
            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(configManager.GetConnectionString());
            return new DataContext(optionsBuilder.Options);
        }
    }
}
