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
    [ApiController]
    [Route("comodities")]
    public class ComodityController : BaseController
    {
        private readonly IComodityService comodityService;
        private readonly IForexService forexService;
        private const string GoldPriceCurrency = "USD";

        public ComodityController(IHttpContextAccessor httpContextAccessor, IComodityService comodityService, IForexService forexService) : base(httpContextAccessor)
        {
            this.comodityService = comodityService;
            this.forexService = forexService;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<ComodityTradeHistoryModel>> Get()
        {
            return Ok(comodityService.GetByUser(GetUserId()));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public IActionResult Add([FromBody] ComodityTradeHistoryModel tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            comodityService.Add(tradeHistory);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public IActionResult Update([FromBody] ComodityTradeHistoryModel tradeHistory)
        {
            tradeHistory.UserIdentityId = GetUserId();
            comodityService.Update(tradeHistory);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!comodityService.UserHasRightToCryptoTrade(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            comodityService.Delete(id);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("comodityType/all")]
        public ActionResult<IEnumerable<ComodityTypeModel>> GetComodityTypes() =>
            Ok(comodityService.GetComodityTypes());

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("comodityUnit/all")]
        public ActionResult<IEnumerable<ComodityUnitModel>> GetComodityUnits() =>
            Ok(comodityService.GetComodityUnits());

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("gold/actualPrice")]
        public async Task<ActionResult<double>> GetCurrentGoldPriceForOunce()
        {
            double exhangeRate = await comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Ok(exhangeRate);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("gold/actualPrice/{currencyCode}")]
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
