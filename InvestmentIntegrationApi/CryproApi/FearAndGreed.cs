using BudgetManager.FinanceDataMining.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BudgetManager.FinanceDataMining.CryproApi
{
    public class FearAndGreed
    {
        private const string ApiUrl = "https://api.alternative.me/fng";
        private readonly HttpClient httpClient;

        public FearAndGreed(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<FearAndGreedReponse> GetActualFearAndGreed() => await this.GetFearAndGreedFrom();

        public async Task<FearAndGreedReponse> GetFearAndGreedFrom(DateTime? from = null)
        {
            string urlDateAppend = string.Empty;

            if (from.HasValue)
            {
                TimeSpan duration = DateTime.Now - from.Value;
                int durationInDays = duration.Days;
                urlDateAppend = $"/?limit={durationInDays}";
            }

            string url = ApiUrl + urlDateAppend;
            return await this.DoApiReqeust(url);
        }

        private async Task<FearAndGreedReponse> DoApiReqeust(string url)
        {
            string response = await this.httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<FearAndGreedReponse>(response);
        }
    }
}
