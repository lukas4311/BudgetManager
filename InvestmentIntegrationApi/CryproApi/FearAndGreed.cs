using System.Net.Http;
using System.Threading.Tasks;

namespace FinanceDataMining.CryproApi
{
    public class FearAndGreed
    {
        private const string ApiUrl = "https://api.alternative.me/fng";
        private readonly HttpClient httpClient;

        public FearAndGreed(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task GetActualFearAndGreed()
        {
            string response = await this.httpClient.GetStringAsync(ApiUrl);
        }
    }
}
