using Asp.Versioning;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling commodity-related operations in the Budget Manager API.
    /// Provides endpoints for managing commodity trades, retrieving commodity types and units, and getting gold prices.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/comodities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class ComodityController : BaseController
    {
        private readonly IComodityService comodityService;
        private readonly IForexService forexService;
        private const string GoldPriceCurrency = "USD";

        /// <summary>
        /// Initializes a new instance of the <see cref="ComodityController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing the current HTTP context.</param>
        /// <param name="comodityService">The service responsible for commodity operations.</param>
        /// <param name="forexService">The service responsible for foreign exchange operations.</param>
        public ComodityController(IHttpContextAccessor httpContextAccessor, IComodityService comodityService, IForexService forexService) : base(httpContextAccessor)
        {
            this.comodityService = comodityService;
            this.forexService = forexService;
        }

        /// <summary>
        /// Retrieves all commodity trade histories for the current user.
        /// </summary>
        /// <returns>A collection of commodity trade histories belonging to the current user.</returns>
        /// <response code="200">Returns the collection of commodity trade histories.</response>
        [HttpGet("all"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<ComodityTradeHistoryModel>> Get()
        {
            return Ok(comodityService.GetByUser(GetUserId()));
        }

        /// <summary>
        /// Adds a new commodity trade history record for the current user.
        /// </summary>
        /// <param name="tradeHistory">The commodity trade history record to add.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The commodity trade history was successfully added.</response>
        [HttpPost, MapToApiVersion("1.0")]
        public IActionResult Add([FromBody] ComodityTradeHistoryModel tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            comodityService.Add(tradeHistory);
            return Ok();
        }

        /// <summary>
        /// Updates an existing commodity trade history record for the current user.
        /// </summary>
        /// <param name="tradeHistory">The commodity trade history record with updated information.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The commodity trade history was successfully updated.</response>
        [HttpPut, MapToApiVersion("1.0")]
        public IActionResult Update([FromBody] ComodityTradeHistoryModel tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            comodityService.Update(tradeHistory);
            return Ok();
        }

        /// <summary>
        /// Deletes a commodity trade history record if the current user has the right to it.
        /// </summary>
        /// <param name="id">The ID of the commodity trade history record to delete.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The commodity trade history was successfully deleted.</response>
        /// <response code="401">The user is not authorized to delete this commodity trade history.</response>
        [HttpDelete, MapToApiVersion("1.0")]
        public IActionResult Delete([FromBody] int id)
        {
            if (!comodityService.UserHasRightToCryptoTrade(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            comodityService.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Retrieves all available commodity types.
        /// </summary>
        /// <returns>A collection of commodity type models.</returns>
        /// <response code="200">Returns the collection of commodity types.</response>
        [HttpGet("comodityType/all"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<ComodityTypeModel>> GetComodityTypes() =>
            Ok(comodityService.GetComodityTypes());

        /// <summary>
        /// Retrieves all available commodity units.
        /// </summary>
        /// <returns>A collection of commodity unit models.</returns>
        /// <response code="200">Returns the collection of commodity units.</response>
        [HttpGet("comodityUnit/all"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<ComodityUnitModel>> GetComodityUnits() =>
            Ok(comodityService.GetComodityUnits());

        /// <summary>
        /// Retrieves the current gold price per ounce in USD.
        /// </summary>
        /// <returns>The current gold price per ounce in USD.</returns>
        /// <response code="200">Returns the current gold price per ounce in USD.</response>
        [HttpGet("gold/actualPrice"), MapToApiVersion("1.0")]
        public async Task<ActionResult<double>> GetCurrentGoldPriceForOunce()
        {
            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Ok(exhangeRate);
        }

        /// <summary>
        /// Retrieves the current gold price per ounce in the specified currency.
        /// </summary>
        /// <param name="currencyCode">The currency code to convert the gold price to.</param>
        /// <returns>The current gold price per ounce in the specified currency.</returns>
        /// <response code="200">Returns the current gold price per ounce in the specified currency.</response>
        /// <response code="400">The specified currency code is not valid.</response>
        [HttpGet("gold/actualPrice/{currencyCode}"), MapToApiVersion("1.0")]
        public async Task<ActionResult<double>> GetCurrentGoldPriceForOunce(string currencyCode)
        {
            double currencyExchangeRate = 1.0;

            if (string.Compare(currencyCode, GoldPriceCurrency, true) != 0)
            {
                currencyExchangeRate = await forexService.GetCurrentExchangeRate(GoldPriceCurrency, currencyCode).ConfigureAwait(false);

                if (currencyExchangeRate == 0)
                    return BadRequest("Currency code is not valid");
            }

            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Ok(exhangeRate * currencyExchangeRate);
        }
    }
}