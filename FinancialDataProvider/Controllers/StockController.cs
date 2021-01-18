using FinanceDataMining.CryproApi;
using FinanceDataMining.Models;
using FinanceDataMining.StockApi;
using FinanceDataMining.StockApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SystemInterface;
using SystemWrapper;

namespace FinancialDataProvider.Controllers
{
    [Route("stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IFinnhubStockApi finnhubStockApi;

        public StockController(IFinnhubStockApi finnhubStockApi)
        {
            this.finnhubStockApi = finnhubStockApi;
        }

        [HttpGet("get")]
        public async Task<StockData> Get()
        {
            StockData stockData = await this.finnhubStockApi.GetRealTimeQuoteData("NASDAQ:AAPL");
            return stockData;
        }

        [HttpGet("getCrypto")]
        public async Task<List<CandleModel>> GetHistoricData()
        {
            CryptoCandleDataApi cryptoCandleDataApi = new CryptoCandleDataApi(new HttpClient());
            return await cryptoCandleDataApi.GetCandlesMonthData("BTC-EUR", new System.DateTime(2019, 1,1));
        }
    }
}
