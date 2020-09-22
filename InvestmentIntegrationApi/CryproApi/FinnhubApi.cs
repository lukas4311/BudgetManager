using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using SystemInterface;
using InvestmentIntegrationApi.Extensions;
using InvestmentIntegrationApi.StockApi.JsonModelDto;

namespace InvestmentIntegrationApi.CryproApi
{
    public partial class FinnhubApi
    {
        private const string Token = "bs445d7rh5rbsfggj800";
        private readonly HttpClient httpClient;
        private readonly IDateTime dateTime;

        public FinnhubApi(HttpClient httpClient, IDateTime dateTime)
        {
            this.httpClient = httpClient;
            this.dateTime = dateTime;
        }

        public async Task GetPreviousMonthCryptoCandles(string cryptoSymbol)
        {
            double seconds = this.dateTime.Now.DateTimeInstance.ConvertToUnixTimestamp();
            double secondsPrevoiusMonth = this.dateTime.Now.AddMonths(-1).DateTimeInstance.ConvertToUnixTimestamp();
            string res = await this.httpClient.GetStringAsync($"https://finnhub.io/api/v1/crypto/candle?symbol=BINANCE:{cryptoSymbol}&resolution=1&from={secondsPrevoiusMonth}&to={seconds}&token={Token}").ConfigureAwait(false);
            CandleData candleData = JsonConvert.DeserializeObject<CandleData>(res);
        }
    }
}
