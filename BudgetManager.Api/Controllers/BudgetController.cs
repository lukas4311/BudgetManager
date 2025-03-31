using System.Collections.Generic;
using Asp.Versioning;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for handling budget-related operations in the Budget Manager API.
    /// Provides endpoints for creating, retrieving, updating, and deleting budgets.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/budgets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class BudgetController : BaseController
    {
        private readonly IBudgetService budgetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetController"/> class.
        /// </summary>
        /// <param name="budgetService">The service responsible for budget operations.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor for accessing the current HTTP context.</param>
        public BudgetController(IBudgetService budgetService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this.budgetService = budgetService;
        }

        /// <summary>
        /// Retrieves all budgets for the current user.
        /// </summary>
        /// <returns>A collection of budget models belonging to the current user.</returns>
        /// <response code="200">Returns the collection of budgets.</response>
        [HttpGet("all"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<BudgetModel>> Get()
        {
            return Ok(budgetService.GetByUserId(GetUserId()));
        }

        /// <summary>
        /// Retrieves a specific budget by ID if the current user has access to it.
        /// </summary>
        /// <param name="id">The ID of the budget to retrieve.</param>
        /// <returns>The budget model with the specified ID.</returns>
        /// <response code="200">Returns the budget with the specified ID.</response>
        /// <response code="401">The user is not authorized to access this budget.</response>
        [HttpGet, MapToApiVersion("1.0")]
        public ActionResult<BudgetModel> Get(int id)
        {
            if (!budgetService.UserHasRightToBudget(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);
            return Ok(budgetService.Get(id));
        }

        /// <summary>
        /// Retrieves all current/active budgets for the current user.
        /// </summary>
        /// <returns>A collection of active budget models for the current user.</returns>
        /// <response code="200">Returns the collection of active budgets.</response>
        [HttpGet("actual"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<BudgetModel>> GetActual()
        {
            return Ok(budgetService.GetActual(GetUserId()));
        }

        /// <summary>
        /// Adds a new budget for the current user.
        /// </summary>
        /// <param name="budgetModel">The budget model to add.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The budget was successfully added.</response>
        [HttpPost, MapToApiVersion("1.0")]
        public IActionResult Add([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = GetUserId();
            budgetService.Add(budgetModel);
            return Ok();
        }

        /// <summary>
        /// Updates an existing budget for the current user.
        /// </summary>
        /// <param name="budgetModel">The budget model with updated information.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The budget was successfully updated.</response>
        [HttpPut, MapToApiVersion("1.0")]
        public IActionResult Update([FromBody] BudgetModel budgetModel)
        {
            budgetModel.UserIdentityId = GetUserId();
            budgetService.Update(budgetModel);
            return Ok();
        }

        /// <summary>
        /// Deletes a budget if the current user has the right to it.
        /// </summary>
        /// <param name="id">The ID of the budget to delete.</param>
        /// <returns>An OK result if the operation is successful.</returns>
        /// <response code="200">The budget was successfully deleted.</response>
        /// <response code="401">The user is not authorized to delete this budget.</response>
        [HttpDelete, MapToApiVersion("1.0")]
        public IActionResult Delete([FromBody] int id)
        {
            if (!budgetService.UserHasRightToBudget(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);
            budgetService.Delete(id);
            return Ok();
        }
    }
}