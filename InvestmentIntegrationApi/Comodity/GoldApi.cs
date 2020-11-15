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

        //https://www.quandl.com/api/v3/datasets/LBMA/GOLD
    }
}
