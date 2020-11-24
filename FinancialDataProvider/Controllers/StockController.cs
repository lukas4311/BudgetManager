using FinanceDataMining.StockApi;
using FinanceDataMining.StockApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
    }
}
