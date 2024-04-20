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
            return Ok(budgetService.GetByUserId(GetUserId()));
        }

        [HttpGet]
        public ActionResult<BudgetModel> Get(int id)
        {
            if (!budgetService.UserHasRightToBudget(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(budgetService.Get(id));
        }

        [HttpGet("actual")]
        public ActionResult<IEnumerable<BudgetModel>> GetActual()
        {
            return Ok(budgetService.GetActual(GetUserId()));
        }

        [HttpPost]
        public IActionResult Add([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = GetUserId();
            budgetService.Add(budgetModel);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = GetUserId();
            budgetService.Update(budgetModel);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!budgetService.UserHasRightToBudget(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            budgetService.Delete(id);
            return Ok();
        }
    }
}
