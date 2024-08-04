using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Domain.Enums;
using BudgetManager.Domain.MessagingContracts;
using BudgetManager.InfluxDbData.Models;
using BudgetManager.Services.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IPublishEndpoint publishEndpoint;

        public StockController(IHttpContextAccessor httpContextAccessor, IStockTickerService stockTickerService, IStockTradeHistoryService stockTradeHistoryService,
            ICompanyProfileService companyProfileService, IStockSplitService stockSplitService, IPublishEndpoint publishEndpoint) : base(httpContextAccessor)
        {
            this.stockTickerService = stockTickerService;
            this.stockTradeHistoryService = stockTradeHistoryService;
            this.companyProfileService = companyProfileService;
            this.stockSplitService = stockSplitService;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Route("stockTicker")]
        public ActionResult<IEnumerable<StockTickerModel>> GetTickers()
        {
            IEnumerable<StockTickerModel> tags = stockTickerService.GetAll();
            return Ok(tags);
        }

        [HttpGet("stockTradeHistory")]
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> Get() => Ok(stockTradeHistoryService.GetAll(GetUserId()));

        [HttpGet("stockTradeHistory/exhangedTo/{forexSymbol}")]
        public async Task<ActionResult<IEnumerable<StockTradeHistoryGetModel>>> Get(ECurrencySymbol forexSymbol) => Ok(await stockTradeHistoryService.GetAll(GetUserId(), forexSymbol));

        [HttpGet("stockTradeHistory/{ticker}")]
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> GetTickerTradeHistory(string ticker) => Ok(stockTradeHistoryService.GetTradeHistory(GetUserId(), ticker));

        [HttpPost("stockTradeHistory")]
        public IActionResult Add([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = GetUserId();
            stockTradeHistoryService.Add(stockTradeHistoryModel);
            return Ok();
        }

        [HttpPut("stockTradeHistory")]
        public IActionResult Update([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = GetUserId();
            stockTradeHistoryService.Update(stockTradeHistoryModel);
            return Ok();
        }

        [HttpDelete("stockTradeHistory")]
        public IActionResult Delete([FromBody] int id)
        {
            if (!stockTradeHistoryService.UserHasRightToStockTradeHistory(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            stockTradeHistoryService.Delete(id);
            return Ok();
        }

        [HttpGet("stock/{ticker}/price")]
        public async Task<ActionResult<IEnumerable<StockPrice>>> GetStockPriceData(string ticker)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return StatusCode(StatusCodes.Status204NoContent);

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker);
            return Ok(data);
        }

        [HttpGet("stock/{ticker}/price/{from}")]
        public async Task<ActionResult<IEnumerable<StockPrice>>> GetStockPriceData(string ticker, DateTime from)
        {
            if (stockTickerService.GetAll().Count(t => string.Compare(t.Ticker, ticker, true) == 0) == 0)
                return StatusCode(StatusCodes.Status204NoContent);

            IEnumerable<StockPrice> data = await stockTradeHistoryService.GetStockPriceHistory(ticker, from);
            return Ok(data);
        }

        [HttpGet("stock/{ticker}/companyProfile")]
        public ActionResult<CompanyProfileModel> GetCompanyProfile(string ticker)
        {
            CompanyProfileModel companyProfile = companyProfileService.Get(c => ticker == c.Symbol).SingleOrDefault();

            if (companyProfile is null)
                return StatusCode(StatusCodes.Status204NoContent);

            return companyProfile;
        }

        [HttpGet("split")]
        public ActionResult<IEnumerable<StockSplitAccumulated>> GetSplitTest()
        {
            IEnumerable<StockSplitAccumulated> splitData = stockSplitService.GetSplitAccumulated();
            return Ok(splitData);
        }

        [HttpPost("brokerReport/{brokerId}")]
        public async Task<IActionResult> UploadReport([FromRoute]int brokerId, IFormFile file)
        {
            using MemoryStream ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] fileBytes = ms.ToArray();
            stockTradeHistoryService.StoreReportToProcess(fileBytes, GetUserId(), brokerId);

            return Ok();
        }

        [HttpPost("tickerRequest")]
        public async Task<IActionResult> TickerRequest(TickerRequest tickerRequest)
        {
            string routingKey = "new_ticker";

            await publishEndpoint.Publish(new TickerRequest
            {
                Ticker = tickerRequest.Ticker,
                UserId = GetUserId()
            }, context => context.SetRoutingKey(routingKey));

            return Ok();
        }

        [HttpGet("trade/monthlygrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedTradesByMonth()
        {
            var data = stockTradeHistoryService.GetAllTradesGroupedByMonth(GetUserId());
            return Ok(data);
        }
    }
}
