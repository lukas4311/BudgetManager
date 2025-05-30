﻿using Asp.Versioning;
using BudgetManager.Api.Enums;
using BudgetManager.Client.FinancialApiClient;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Domain.Enums;
using BudgetManager.Domain.MessagingContracts;
using BudgetManager.Services.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FinancialClient = BudgetManager.Client.FinancialApiClient.FinancialClient;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller for handling stock-related operations, including stock tickers, trade history, pricing data, and company profiles.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class StockController : BaseController
    {
        private readonly IStockTickerService stockTickerService;
        private readonly IStockTradeHistoryService stockTradeHistoryService;
        private readonly ICompanyProfileService companyProfileService;
        private readonly IStockSplitService stockSplitService;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly HttpClient finHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="stockTickerService">Service for stock ticker management.</param>
        /// <param name="stockTradeHistoryService">Service for managing stock trade history.</param>
        /// <param name="companyProfileService">Service for retrieving company profiles.</param>
        /// <param name="stockSplitService">Service for managing stock splits.</param>
        /// <param name="publishEndpoint">MassTransit endpoint for event publishing.</param>
        public StockController(
            IHttpContextAccessor httpContextAccessor,
            IStockTickerService stockTickerService,
            IStockTradeHistoryService stockTradeHistoryService,
            ICompanyProfileService companyProfileService,
            IStockSplitService stockSplitService,
            IPublishEndpoint publishEndpoint,
            IHttpClientFactory httpClientFactory) : base(httpContextAccessor)
        {
            this.stockTickerService = stockTickerService;
            this.stockTradeHistoryService = stockTradeHistoryService;
            this.companyProfileService = companyProfileService;
            this.stockSplitService = stockSplitService;
            this.publishEndpoint = publishEndpoint;
            this.finHttpClient = httpClientFactory.CreateClient(nameof(HttpClientKeys.FinApi));
        }

        /// <summary>
        /// Retrieves all stock tickers available in the system.
        /// </summary>
        [HttpGet("stockTicker"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<StockTickerModel>> GetTickers()
        {
            return Ok(stockTickerService.GetAll());
        }

        /// <summary>
        /// Updates metadata for a specific stock ticker.
        /// </summary>
        [HttpPut("stockTicker/{tickerId}/metadata"), MapToApiVersion("1.0")]
        public ActionResult UpdateTickerMetadata(int tickerId, [FromBody] string metadata)
        {
            stockTickerService.UpdateTickerMetadata(tickerId, metadata);
            return Ok();
        }

        /// <summary>
        /// Updates details of a specific stock ticker.
        /// </summary>
        [HttpPut("stockTicker/{tickerId}"), MapToApiVersion("1.0")]
        public ActionResult UpdateTicker(int tickerId, StockTickerModel stockTickerModel)
        {
            stockTickerModel.Id = tickerId;
            stockTickerService.UpdateTicker(stockTickerModel);
            return Ok();
        }

        /// <summary>
        /// Retrieves all stock trade history entries for the current user.
        /// </summary>
        [HttpGet("stockTradeHistory"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> Get()
            => Ok(stockTradeHistoryService.GetAll(GetUserId()));

        /// <summary>
        /// Retrieves stock trade history converted to a specified foreign currency.
        /// </summary>
        [HttpGet("stockTradeHistory/exhangedTo/{forexSymbol}"), MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<StockTradeHistoryGetModel>>> Get(ECurrencySymbol forexSymbol)
            => Ok(await stockTradeHistoryService.GetAll(GetUserId(), forexSymbol));

        /// <summary>
        /// Retrieves stock trade history for a specific ticker.
        /// </summary>
        [HttpGet("stockTradeHistory/{ticker}"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> GetTickerTradeHistory(string ticker)
            => Ok(stockTradeHistoryService.GetTradeHistory(GetUserId(), ticker));

        /// <summary>
        /// Adds a new stock trade history entry.
        /// </summary>
        [HttpPost("stockTradeHistory"), MapToApiVersion("1.0")]
        public IActionResult Add([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = GetUserId();
            stockTradeHistoryService.Add(stockTradeHistoryModel);
            return Ok();
        }

        /// <summary>
        /// Updates an existing stock trade history entry.
        /// </summary>
        [HttpPut("stockTradeHistory"), MapToApiVersion("1.0")]
        public IActionResult Update([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = GetUserId();
            stockTradeHistoryService.Update(stockTradeHistoryModel);
            return Ok();
        }

        /// <summary>
        /// Deletes a stock trade history entry if the user has the right permissions.
        /// </summary>
        [HttpDelete("stockTradeHistory"), MapToApiVersion("1.0")]
        public IActionResult Delete([FromBody] int id)
        {
            if (!stockTradeHistoryService.UserHasRightToStockTradeHistory(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            stockTradeHistoryService.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Retrieves stock price history for a specific ticker.
        /// </summary>
        [HttpGet("stock/{ticker}/price"), MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<InfluxDbData.Models.StockPrice>>> GetStockPriceData(string ticker)
        {
            if (!stockTickerService.GetAll().Any(t => string.Equals(t.Ticker, ticker, StringComparison.OrdinalIgnoreCase)))
                return StatusCode(StatusCodes.Status204NoContent);

            return Ok(await stockTradeHistoryService.GetStockPriceHistory(ticker));
        }

        /// <summary>
        /// Retrieves the company profile for a given stock ticker.
        /// </summary>
        [HttpGet("stock/{ticker}/companyProfile"), MapToApiVersion("1.0")]
        public ActionResult<CompanyProfileModel> GetCompanyProfile(string ticker)
        {
            CompanyProfileModel companyProfile = companyProfileService.Get(c => c.Symbol == ticker).SingleOrDefault();

            if (companyProfile is null)
                return StatusCode(StatusCodes.Status204NoContent);

            return companyProfile;
        }

        /// <summary>
        /// Uploads a broker report file for processing.
        /// </summary>
        [HttpPost("brokerReport/{brokerId}"), MapToApiVersion("1.0")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadReport([FromRoute] int brokerId, IFormFile file)
        {
            using MemoryStream ms = new MemoryStream();
            await file.CopyToAsync(ms);
            stockTradeHistoryService.StoreReportToProcess(ms.ToArray(), GetUserId(), brokerId);
            return Ok();
        }

        /// <summary>
        /// Publishes a request for a new stock ticker.
        /// </summary>
        [HttpPost("tickerRequest"), MapToApiVersion("1.0")]
        public async Task<IActionResult> TickerRequest(TickerRequest tickerRequest)
        {
            await publishEndpoint.Publish(new TickerRequest { Ticker = tickerRequest.Ticker, UserId = GetUserId() }, context => context.SetRoutingKey("new_ticker"));
            return Ok();
        }

        /// <summary>
        /// Retrieves all trades grouped by month for the current user.
        /// </summary>
        /// <returns>A list of trades grouped by month.</returns>
        [HttpGet("trade/monthlygrouped"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedTradesByMonth()
        {
            IEnumerable<TradesGroupedMonth> data = stockTradeHistoryService.GetAllTradesGroupedByMonth(GetUserId());
            return Ok(data);
        }

        /// <summary>
        /// Retrieves all trades grouped by ticker and trade date for the current user.
        /// </summary>
        /// <returns>A list of trades grouped by ticker and trade date.</returns>
        [HttpGet("trade/tradedategrouped"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<TradeGroupedTradeTime>> GetGroupedByTickerAndTradeDate()
        {
            IEnumerable<TradeGroupedTradeTime> data = stockTradeHistoryService.GetAllTradesGroupedByTradeDate(GetUserId());
            return Ok(data);
        }

        /// <summary>
        /// Retrieves all trades grouped by ticker for the current user.
        /// </summary>
        /// <returns>A list of trades grouped by ticker.</returns>
        [HttpGet("trade/tickergrouped"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<TradeGroupedTicker>> GetGroupedByTicker()
        {
            IEnumerable<TradeGroupedTicker> data = stockTradeHistoryService.GetAllTradesGroupedByTicker(GetUserId());
            return Ok(data);
        }

        [HttpGet("trade/tickergrouped-in-currency"), MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<TradeGroupedTickerWithProfitLoss>>> GetStockTradesInCurrency([FromQuery]string currency)
        {
            FinancialClient client = new FinancialClient(finHttpClient);
            IEnumerable<TradeGroupedTickerWithProfitLoss> data = await stockTradeHistoryService.GetAllTradesGroupedByTickerWithProfitInfo(GetUserId(), currency);
            return Ok(data);
        }
    }
}
