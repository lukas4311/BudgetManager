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

        [HttpGet("all")]
        public ActionResult<IEnumerable<ComodityTradeHistoryModel>> Get()
        {
            return Ok(this.comodityService.GetByUser(this.GetUserId()));
        }

        [HttpPost]
        public IActionResult Add([FromBody] ComodityTradeHistoryModel tradeHistory)
        {
            tradeHistory.UserIdentityId = this.GetUserId();
            this.comodityService.Add(tradeHistory);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update([FromBody] ComodityTradeHistoryModel tradeHistory)
        {
            tradeHistory.UserIdentityId = this.GetUserId();
            this.comodityService.Update(tradeHistory);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!this.comodityService.UserHasRightToCryptoTrade(id, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.comodityService.Delete(id);
            return Ok();
        }

        [HttpGet("comodityType/all")]
        public ActionResult<IEnumerable<ComodityTypeModel>> GetComodityTypes() =>
            this.Ok(this.comodityService.GetComodityTypes());

        [HttpGet("comodityUnit/all")]
        public ActionResult<IEnumerable<ComodityUnitModel>> GetComodityUnits() =>
            this.Ok(this.comodityService.GetComodityUnits());

        [HttpGet("gold/actualPrice")]
        public async Task<ActionResult<double>> GetCurrentGoldPriceForOunce()
        {
            double exhangeRate = await this.comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Ok(exhangeRate);
        }

        [HttpGet("gold/actualPrice/{currencyCode}")]
        public async Task<ActionResult<double>> GetCurrentGoldPriceForOunce(string currencyCode)
        {
            double currencyExchangeRate = 1.0;

            if (string.Compare(currencyCode, GoldPriceCurrency, true) != 0)
            {
                currencyExchangeRate = await this.forexService.GetCurrentExchangeRate(GoldPriceCurrency, currencyCode).ConfigureAwait(false);

                if (currencyExchangeRate == 0)
                    throw new ArgumentException("Currency code is not valid");
            }

            double exhangeRate = await this.comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Ok(exhangeRate * currencyExchangeRate);
        }

        //[HttpGet("test")]
        //public IActionResult Test()
        //{
        //    this.comodityService.Delete(5);

        //    return Ok();
        //}
    }
}
