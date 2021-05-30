using FinanceDataMining.Models;
using System.Net.Http;
using System.Text.Json;
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

        public async Task<FearAndGreedReponse> GetActualFearAndGreed()
        {
            string response = await this.httpClient.GetStringAsync(ApiUrl);
            FearAndGreedReponse responseModel = JsonSerializer.Deserialize<FearAndGreedReponse>(response);
            return responseModel;
        }
    }
}
