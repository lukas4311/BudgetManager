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
        private readonly IStockTradeHistoryService stockTradeHistoryService;

        public StockController(IHttpContextAccessor httpContextAccessor, IStockTickerService stockTickerService, IStockTradeHistoryService stockTradeHistoryService) : base(httpContextAccessor)
        {
            this.stockTickerService = stockTickerService;
            this.stockTradeHistoryService = stockTradeHistoryService;
        }

        [HttpGet]
        [Route("stockTicker")]
        public ActionResult<IEnumerable<StockTickerModel>> GetTickers()
        {
            IEnumerable<StockTickerModel> tags = this.stockTickerService.GetAll();
            return Ok(tags);
        }

        [HttpGet("stockTradeHistory")]
        public ActionResult<IEnumerable<OtherInvestmentModel>> Get()
        {
            return Ok(this.stockTradeHistoryService.GetAll(this.GetUserId()));
        }

        [HttpPost("stockTradeHistory")]
        public IActionResult Add([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = this.GetUserId();
            this.stockTradeHistoryService.Add(stockTradeHistoryModel);
            return Ok();
        }

        [HttpPut("stockTradeHistory")]
        public IActionResult Update([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = this.GetUserId();
            this.stockTradeHistoryService.Update(stockTradeHistoryModel);
            return Ok();
        }

        [HttpDelete("stockTradeHistory")]
        public IActionResult Delete([FromBody] int id)
        {
            if (!this.stockTradeHistoryService.UserHasRightToPayment(id, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.stockTradeHistoryService.Delete(id);
            return Ok();
        }
    }
}
