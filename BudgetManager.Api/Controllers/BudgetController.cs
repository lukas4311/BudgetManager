using System.Collections.Generic;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("budget")]
    public class BudgetController : BaseController
    {
        private readonly IBudgetService budgetService;

        public BudgetController(IBudgetService budgetService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this.budgetService = budgetService;
        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<BudgetModel>> Get()
        {
            return Ok(this.budgetService.GetByUserId(this.GetUserId()));
        }

        [HttpGet("get")]
        public ActionResult<BudgetModel> Get(int id)
        {
            if (!this.budgetService.UserHasRightToBudget(id, this.GetUserId()))
                return this.StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(this.budgetService.Get(id));
        }

        [HttpGet("getActual")]
        public ActionResult<IEnumerable<BudgetModel>> GetActual()
        {
            return Ok(this.budgetService.GetActual(this.GetUserId()));
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] BudgetModel budgetModel)
        {
            if (budgetModel.UserIdentityId != this.GetUserId())
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.budgetService.Add(budgetModel);
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] BudgetModel budgetModel)
        {
            if (budgetModel.UserIdentityId != this.GetUserId())
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.budgetService.Update(budgetModel);
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromBody] int id)
        {
            if (!this.budgetService.UserHasRightToBudget(id, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.budgetService.Delete(id);
            return Ok();
        }
    }
}
