using Microsoft.AspNetCore.Mvc;

namespace FinancialDataProvider.Controllers
{
    public class StockController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
