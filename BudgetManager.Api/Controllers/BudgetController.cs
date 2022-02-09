using System.Collections.Generic;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("budgets")]
    public class BudgetController : BaseController
    {
        private readonly IBudgetService budgetService;

        public BudgetController(IBudgetService budgetService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this.budgetService = budgetService;
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<BudgetModel>> Get()
        {
            return Ok(this.budgetService.GetByUserId(this.GetUserId()));
        }

        [HttpGet]
        public ActionResult<BudgetModel> Get(int id)
        {
            if (!this.budgetService.UserHasRightToBudget(id, this.GetUserId()))
                return this.StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(this.budgetService.Get(id));
        }

        [HttpGet("actual")]
        public ActionResult<IEnumerable<BudgetModel>> GetActual()
        {
            return Ok(this.budgetService.GetActual(this.GetUserId()));
        }

        [HttpPost]
        public IActionResult Add([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = this.GetUserId();
            this.budgetService.Add(budgetModel);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = this.GetUserId();
            this.budgetService.Update(budgetModel);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!this.budgetService.UserHasRightToBudget(id, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.budgetService.Delete(id);
            return Ok();
        }
    }
}
