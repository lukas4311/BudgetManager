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
    /// <summary>
    /// Controller for handling stock-related operations, including stock tickers, trade history, pricing data, and company profiles.
    /// </summary>
    [ApiController]
    [Route("stock")]
    public class StockController : BaseController
    {
        private readonly IStockTickerService stockTickerService;
        private readonly IStockTradeHistoryService stockTradeHistoryService;
        private readonly ICompanyProfileService companyProfileService;
        private readonly IStockSplitService stockSplitService;
        private readonly IPublishEndpoint publishEndpoint;

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
            IPublishEndpoint publishEndpoint) : base(httpContextAccessor)
        {
            this.stockTickerService = stockTickerService;
            this.stockTradeHistoryService = stockTradeHistoryService;
            this.companyProfileService = companyProfileService;
            this.stockSplitService = stockSplitService;
            this.publishEndpoint = publishEndpoint;
        }

        /// <summary>
        /// Retrieves all stock tickers available in the system.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("stockTicker")]
        public ActionResult<IEnumerable<StockTickerModel>> GetTickers()
        {
            return Ok(stockTickerService.GetAll());
        }

        /// <summary>
        /// Updates metadata for a specific stock ticker.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("stockTicker/{tickerId}/metadata")]
        public ActionResult UpdateTickerMetadata(int tickerId, [FromBody] string metadata)
        {
            stockTickerService.UpdateTickerMetadata(tickerId, metadata);
            return Ok();
        }

        /// <summary>
        /// Updates details of a specific stock ticker.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("stockTicker/{tickerId}")]
        public ActionResult UpdateTicker(int tickerId, StockTickerModel stockTickerModel)
        {
            stockTickerModel.Id = tickerId;
            stockTickerService.UpdateTicker(stockTickerModel);
            return Ok();
        }

        /// <summary>
        /// Retrieves all stock trade history entries for the current user.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("stockTradeHistory")]
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> Get()
            => Ok(stockTradeHistoryService.GetAll(GetUserId()));

        /// <summary>
        /// Retrieves stock trade history converted to a specified foreign currency.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("stockTradeHistory/exhangedTo/{forexSymbol}")]
        public async Task<ActionResult<IEnumerable<StockTradeHistoryGetModel>>> Get(ECurrencySymbol forexSymbol)
            => Ok(await stockTradeHistoryService.GetAll(GetUserId(), forexSymbol));

        /// <summary>
        /// Retrieves stock trade history for a specific ticker.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("stockTradeHistory/{ticker}")]
        public ActionResult<IEnumerable<StockTradeHistoryGetModel>> GetTickerTradeHistory(string ticker)
            => Ok(stockTradeHistoryService.GetTradeHistory(GetUserId(), ticker));

        /// <summary>
        /// Adds a new stock trade history entry.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("stockTradeHistory")]
        public IActionResult Add([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = GetUserId();
            stockTradeHistoryService.Add(stockTradeHistoryModel);
            return Ok();
        }

        /// <summary>
        /// Updates an existing stock trade history entry.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut("stockTradeHistory")]
        public IActionResult Update([FromBody] StockTradeHistoryModel stockTradeHistoryModel)
        {
            stockTradeHistoryModel.UserIdentityId = GetUserId();
            stockTradeHistoryService.Update(stockTradeHistoryModel);
            return Ok();
        }

        /// <summary>
        /// Deletes a stock trade history entry if the user has the right permissions.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("stockTradeHistory")]
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
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("stock/{ticker}/price")]
        public async Task<ActionResult<IEnumerable<StockPrice>>> GetStockPriceData(string ticker)
        {
            if (!stockTickerService.GetAll().Any(t => string.Equals(t.Ticker, ticker, StringComparison.OrdinalIgnoreCase)))
                return StatusCode(StatusCodes.Status204NoContent);

            return Ok(await stockTradeHistoryService.GetStockPriceHistory(ticker));
        }

        /// <summary>
        /// Retrieves the company profile for a given stock ticker.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("stock/{ticker}/companyProfile")]
        public ActionResult<CompanyProfileModel> GetCompanyProfile(string ticker)
        {
            var companyProfile = companyProfileService.Get(c => c.Symbol == ticker).SingleOrDefault();

            if (companyProfile is null)
                return StatusCode(StatusCodes.Status204NoContent);

            return companyProfile;
        }

        /// <summary>
        /// Uploads a broker report file for processing.
        /// </summary>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("brokerReport/{brokerId}")]
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
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("tickerRequest")]
        public async Task<IActionResult> TickerRequest(TickerRequest tickerRequest)
        {
            await publishEndpoint.Publish(new TickerRequest { Ticker = tickerRequest.Ticker, UserId = GetUserId() }, context => context.SetRoutingKey("new_ticker"));
            return Ok();
        }

        /// <summary>
        /// Retrieves all trades grouped by month for the current user.
        /// </summary>
        /// <returns>A list of trades grouped by month.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/monthlygrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedTradesByMonth()
        {
            var data = stockTradeHistoryService.GetAllTradesGroupedByMonth(GetUserId());
            return Ok(data);
        }

        /// <summary>
        /// Retrieves all trades grouped by ticker and trade date for the current user.
        /// </summary>
        /// <returns>A list of trades grouped by ticker and trade date.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/tradedategrouped")]
        public ActionResult<IEnumerable<TradeGroupedTradeTime>> GetGroupedByTickerAndTradeDate()
        {
            var data = stockTradeHistoryService.GetAllTradesGroupedByTradeDate(GetUserId());
            return Ok(data);
        }

        /// <summary>
        /// Retrieves all trades grouped by ticker for the current user.
        /// </summary>
        /// <returns>A list of trades grouped by ticker.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/tickergrouped")]
        public ActionResult<IEnumerable<TradeGroupedTicker>> GetGroupedByTicker()
        {
            var data = stockTradeHistoryService.GetAllTradesGroupedByTicker(GetUserId());
            return Ok(data);
        }
    }
}
