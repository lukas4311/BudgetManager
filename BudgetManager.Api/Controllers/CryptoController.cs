using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Domain.DTOs.Queries;
using BudgetManager.Repository;
using BudgetManager.Services;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling cryptocurrency-related operations in the Budget Manager API.
    /// Provides endpoints for managing crypto trades, exchange rates, and broker reports.
    /// </summary>
    [ApiController]
    [Route("cryptos")]
    public class CryptoController : BaseController
    {
        private readonly ICryptoService cryptoService;
        private readonly IForexService forexService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing the current HTTP context.</param>
        /// <param name="cryptoService">The service responsible for cryptocurrency operations.</param>
        /// <param name="forexService">The service responsible for foreign exchange operations.</param>
        public CryptoController(IHttpContextAccessor httpContextAccessor, ICryptoService cryptoService, IForexService forexService) : base(httpContextAccessor)
        {
            this.cryptoService = cryptoService;
            this.forexService = forexService;
        }

        /// <summary>
        /// Retrieves all trade histories for the current user.
        /// </summary>
        /// <returns>A collection of trade histories belonging to the current user.</returns>
        /// <response code="200">Returns the collection of trade histories.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<TradeHistory>> Get()
        {
            return Ok(cryptoService.GetByUser(GetUserId()));
        }

        /// <summary>
        /// Adds a new trade history record for the current user.
        /// </summary>
        /// <param name="tradeHistory">The trade history record to add.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The trade history was successfully added.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public IActionResult Add([FromBody] TradeHistory tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            cryptoService.Add(tradeHistory);
            return Ok();
        }

        /// <summary>
        /// Updates an existing trade history record for the current user.
        /// </summary>
        /// <param name="tradeHistory">The trade history record with updated information.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The trade history was successfully updated.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public IActionResult Update([FromBody] TradeHistory tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            cryptoService.Update(tradeHistory);
            return Ok();
        }

        /// <summary>
        /// Deletes a trade history record if the current user has the right to it.
        /// </summary>
        /// <param name="id">The ID of the trade history record to delete.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The trade history was successfully deleted.</response>
        /// <response code="401">The user is not authorized to delete this trade history.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!cryptoService.UserHasRightToCryptoTrade(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            cryptoService.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Retrieves the current exchange rate between two currencies.
        /// Attempts to get the rate from forex first, then from crypto if forex returns 0.
        /// </summary>
        /// <param name="fromCurrency">The source currency code.</param>
        /// <param name="toCurrency">The target currency code.</param>
        /// <returns>The current exchange rate as a double value.</returns>
        /// <response code="200">Returns the current exchange rate.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("actualExchangeRate/{fromCurrency}/{toCurrency}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency)
        {
            double exhangeRate = await forexService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            if (exhangeRate == 0)
                exhangeRate = await cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency).ConfigureAwait(false);

            return Ok(exhangeRate);
        }

        /// <summary>
        /// Retrieves the exchange rate between two currencies at a specific date.
        /// </summary>
        /// <param name="fromCurrency">The source currency code.</param>
        /// <param name="toCurrency">The target currency code.</param>
        /// <param name="atDate">The date for which to retrieve the exchange rate.</param>
        /// <returns>The exchange rate at the specified date as a double value.</returns>
        /// <response code="200">Returns the exchange rate at the specified date.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("exchangeRate/{fromCurrency}/{toCurrency}/{atDate}")]
        public async Task<ActionResult<double>> GetCurrentExchangeRate(string fromCurrency, string toCurrency, DateTime atDate)
        {
            double exhangeRate = await cryptoService.GetCurrentExchangeRate(fromCurrency, toCurrency, atDate).ConfigureAwait(false);
            return Ok(exhangeRate);
        }

        /// <summary>
        /// Retrieves a specific trade history record by ID for the current user.
        /// </summary>
        /// <param name="tradeId">The ID of the trade history record to retrieve.</param>
        /// <returns>The trade history record with the specified ID.</returns>
        /// <response code="200">Returns the trade history record.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("tradeDetail/{tradeId}")]
        public ActionResult<TradeHistory> Get(int tradeId)
        {
            return Ok(cryptoService.Get(tradeId, GetUserId()));
        }

        /// <summary>
        /// Retrieves all available cryptocurrency tickers.
        /// </summary>
        /// <returns>A collection of cryptocurrency ticker models.</returns>
        /// <response code="200">Returns the collection of cryptocurrency tickers.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("tickers")]
        public ActionResult<IEnumerable<CryptoTickerModel>> GetTickers() => Ok(cryptoService.GetAllTickers());

        /// <summary>
        /// Uploads a broker report file for processing.
        /// </summary>
        /// <param name="brokerId">The ID of the broker associated with the report.</param>
        /// <param name="file">The broker report file to upload.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The broker report was successfully uploaded for processing.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("brokerReport/{brokerId}")]
        public async Task<IActionResult> UploadReport([FromRoute] int brokerId, IFormFile file)
        {
            using MemoryStream ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] fileBytes = ms.ToArray();
            cryptoService.StoreReportToProcess(fileBytes, GetUserId(), brokerId);

            return Ok();
        }

        /// <summary>
        /// Retrieves trades grouped by month for the current user.
        /// </summary>
        /// <returns>A collection of trades grouped by month.</returns>
        /// <response code="200">Returns the collection of trades grouped by month.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/monthlygrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedTradesByMonth()
        {
            var data = cryptoService.GetAllTradesGroupedByMonth(GetUserId());
            return Ok(data);
        }

        /// <summary>
        /// Retrieves trades grouped by ticker and trade date for the current user.
        /// </summary>
        /// <returns>A collection of trades grouped by ticker and trade date.</returns>
        /// <response code="200">Returns the collection of trades grouped by ticker and trade date.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/tradedategrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedByTickerAndTradeDate()
        {
            var data = cryptoService.GetAllTradesGroupedByTradeDate(GetUserId());
            return Ok(data);
        }

        /// <summary>
        /// Retrieves trades grouped by ticker for the current user.
        /// </summary>
        /// <returns>A collection of trades grouped by ticker.</returns>
        /// <response code="200">Returns the collection of trades grouped by ticker.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("trade/tickergrouped")]
        public ActionResult<IEnumerable<TradesGroupedMonth>> GetGroupedByTicker()
        {
            var data = cryptoService.GetAllTradesGroupedByTicker(GetUserId());
            return Ok(data);
        }
    }
}