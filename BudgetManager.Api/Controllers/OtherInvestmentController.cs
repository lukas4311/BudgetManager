using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("otherInvestment")]
    public class OtherInvestmentController : BaseController
    {
        private readonly IOtherInvestmentService otherInvestmentService;

        public OtherInvestmentController(IHttpContextAccessor httpContextAccessor, IOtherInvestmentService otherInvestmentService) : base(httpContextAccessor)
        {
            this.otherInvestmentService = otherInvestmentService;
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
            this.otherInvestmentService.Add(otherInvestment);
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
    }
}
