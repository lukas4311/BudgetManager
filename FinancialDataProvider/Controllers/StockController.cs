using FinanceDataMining.StockApi;
using Microsoft.AspNetCore.Mvc;

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
        public string Index()
        {
            return "Ahoj";
        }
    }
}
