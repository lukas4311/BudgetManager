using BudgetManager.Domain.DTOs;
using BudgetManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("comodities")]
    public class ComodityController : BaseController
    {
        private readonly IComodityService comodityService;

        public ComodityController(IHttpContextAccessor httpContextAccessor, IComodityService comodityService) : base(httpContextAccessor)
        {
            this.comodityService = comodityService;
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
        public async Task<ActionResult<double>> GetCurrentExchangeRate()
        {
            double exhangeRate = await this.comodityService.GetCurrentGoldPriceForOunce().ConfigureAwait(false);
            return Ok(exhangeRate);
        }
    }
}
