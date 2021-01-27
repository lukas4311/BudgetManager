using FinanceDataMining.CryproApi;
using FinanceDataMining.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestingConsole
{
    public class DataDownloader
    {
        const string organizationId = "8f46f33452affe4a";
        const string bucket = "Crypto";

        public async Task<List<CandleModel>> DownloadData(CryptoTicker cryptoTicker, DateTime from)
        {
            CryptoWatch cryptoCandleDataApi = new CryptoWatch(new HttpClient());
            return await cryptoCandleDataApi.GetCandlesDataFrom(cryptoTicker.ToString(), from).ConfigureAwait(false);
        }
    }
}
