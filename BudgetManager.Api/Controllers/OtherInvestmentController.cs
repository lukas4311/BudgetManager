using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("otherInvestment")]
    public class OtherInvestmentController : BaseController
    {
        private readonly IOtherInvestmentService otherInvestmentService;
        private readonly IOtherInvestmentBalaceHistoryService otherInvestmentBalaceHistoryService;

        public OtherInvestmentController(IHttpContextAccessor httpContextAccessor, IOtherInvestmentService otherInvestmentService,
            IOtherInvestmentBalaceHistoryService otherInvestmentBalaceHistoryService) : base(httpContextAccessor)
        {
            this.otherInvestmentService = otherInvestmentService;
            this.otherInvestmentBalaceHistoryService = otherInvestmentBalaceHistoryService;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<OtherInvestmentModel>> Get()
        {
            return Ok(this.otherInvestmentService.GetAll(this.GetUserId()));
        }

        [HttpPost]
        public IActionResult Add([FromBody] OtherInvestmentModel otherInvestment)
        {
            otherInvestment.UserIdentityId = this.GetUserId();
            var id = this.otherInvestmentService.Add(otherInvestment);
            this.otherInvestmentBalaceHistoryService.Add(new OtherInvestmentBalaceHistoryModel
            {
                Balance = otherInvestment.OpeningBalance,
                Date = otherInvestment.Created,
                OtherInvestmentId = id
            });
            return Ok();
        }

        [HttpPut]
        public IActionResult Update([FromBody] OtherInvestmentModel otherInvestment)
        {
            otherInvestment.UserIdentityId = this.GetUserId();
            this.otherInvestmentService.Update(otherInvestment);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!this.otherInvestmentService.UserHasRightToPayment(id, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.otherInvestmentService.Delete(id);
            return Ok();
        }

        [HttpPost("{otherInvestmentId}/balanceHistory")]
        public IActionResult AddHistoryBalance(int otherInvestmentId, [FromBody] OtherInvestmentBalaceHistoryModel otherInvestmentBalaceHistory)
        {
            otherInvestmentBalaceHistory.Id = otherInvestmentId;
            this.otherInvestmentBalaceHistoryService.Add(otherInvestmentBalaceHistory);
            return Ok();
        }

        [HttpPut("/balanceHistory")]
        public IActionResult UpdateHistoryBalance([FromBody] OtherInvestmentBalaceHistoryModel otherInvestmentBalaceHistory)
        {
            this.otherInvestmentBalaceHistoryService.Update(otherInvestmentBalaceHistory);
            return Ok();
        }

        [HttpDelete("/balanceHistory")]
        public IActionResult DeleteHistoryBalance([FromBody] int id)
        {
            if (!this.otherInvestmentService.UserHasRightToPayment(id, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.otherInvestmentService.Delete(id);
            return Ok();
        }
    }
}
