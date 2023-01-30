using BudgetManager.Domain.DTOs;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> Get() => Ok(this.stockTradeHistoryService.GetAll(this.GetUserId()));

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

        [HttpGet("stock/{ticker}/price")]
        public async Task<ActionResult<IEnumerable<StockPrice>>> GetStockPriceData(string ticker)
        {
            if (this.stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return StatusCode(StatusCodes.Status204NoContent);

            IEnumerable<StockPrice> data = await this.stockTradeHistoryService.GetStockPriceHistory(ticker);
            return Ok(data);
        }

        [HttpGet("stock/{ticker}/price/{from}")]
        public async Task<ActionResult<IEnumerable<StockPrice>>> GetStockPriceData(string ticker, DateTime from)
        {
            if (this.stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return StatusCode(StatusCodes.Status204NoContent);

            IEnumerable<StockPrice> data = await this.stockTradeHistoryService.GetStockPriceHistory(ticker, from);
            return Ok(data);
        }
    }
}
