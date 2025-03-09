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

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<BudgetModel>> Get()
        {
            return Ok(budgetService.GetByUserId(GetUserId()));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public ActionResult<BudgetModel> Get(int id)
        {
            if (!budgetService.UserHasRightToBudget(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(budgetService.Get(id));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("actual")]
        public ActionResult<IEnumerable<BudgetModel>> GetActual()
        {
            return Ok(budgetService.GetActual(GetUserId()));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public IActionResult Add([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = GetUserId();
            budgetService.Add(budgetModel);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public IActionResult Update([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = GetUserId();
            budgetService.Update(budgetModel);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
