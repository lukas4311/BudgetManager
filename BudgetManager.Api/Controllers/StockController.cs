using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("stock")]
    public class StockController : BaseController
    {
        private readonly IStockTickerService stockTickerService;

        public StockController(IHttpContextAccessor httpContextAccessor, IStockTickerService stockTickerService) : base(httpContextAccessor)
        {
            this.stockTickerService = stockTickerService;
        }

        [HttpGet]
        [Route("stockTicker")]
        public ActionResult<IEnumerable<StockTickerModel>> GetTickers()
        {
            IEnumerable<StockTickerModel> tags = this.stockTickerService.GetAll();
            return Ok(tags);
        }
    }
}
