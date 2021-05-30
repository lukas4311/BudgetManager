using System.Net.Http;

namespace FinanceDataMining.Comodity
{
    public class GoldApi
    {
        private readonly HttpClient httpClient;

        public GoldApi(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
    }
}
