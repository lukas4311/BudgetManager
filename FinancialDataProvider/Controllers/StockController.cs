using Microsoft.AspNetCore.Mvc;

namespace FinancialDataProvider.Controllers
{
    [Route("stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet("get")]
        public string Index()
        {
            return "Ahoj";
        }
    }
}
