using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.Enums;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly ICompanyProfileService companyProfileService;
        private readonly IStockSplitService stockSplitService;

        public StockController(IHttpContextAccessor httpContextAccessor, IStockTickerService stockTickerService, IStockTradeHistoryService stockTradeHistoryService, 
            ICompanyProfileService companyProfileService, IStockSplitService stockSplitService) : base(httpContextAccessor)
        {
            this.stockTickerService = stockTickerService;
            this.stockTradeHistoryService = stockTradeHistoryService;
            this.companyProfileService = companyProfileService;
            this.stockSplitService = stockSplitService;
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

        [HttpGet("stockTradeHistory/exhangedTo/{forexSymbol}")]
        public async Task<ActionResult<IEnumerable<StockTradeHistoryGetModel>>> Get(ECurrencySymbol forexSymbol) => Ok(await this.stockTradeHistoryService.GetAll(this.GetUserId(), forexSymbol));

        [HttpGet("stockTradeHistory/{ticker}")]
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> GetTickerTradeHistory(string ticker) => Ok(this.stockTradeHistoryService.GetTradeHistory(this.GetUserId(), ticker));

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
            if (!this.stockTradeHistoryService.UserHasRightToStockTradeHistory(id, this.GetUserId()))
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

        [HttpGet("stock/{ticker}/companyProfile")]
        public ActionResult<CompanyProfileModel> GetCompanyProfile(string ticker)
        {
            CompanyProfileModel companyProfile = this.companyProfileService.Get(c => ticker == c.Symbol).SingleOrDefault();
            
            if (companyProfile is null)
                return StatusCode(StatusCodes.Status204NoContent);

            return companyProfile;
        }

        [HttpGet("split")]
        public IActionResult GetSplitTest()
        {
            this.stockSplitService.GetSplitAccumulated();
            return Ok();
        }

        [HttpPost("brokerReport")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            using MemoryStream ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] fileBytes = ms.ToArray();
            this.stockTradeHistoryService.StoreReportToProcess(fileBytes, this.GetUserId());

            return Ok();
        }
    }
}
