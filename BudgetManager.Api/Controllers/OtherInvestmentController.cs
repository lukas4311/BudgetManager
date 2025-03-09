using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing other investment operations in the Budget Manager application.
    /// </summary>
    [ApiController]
    [Route("otherInvestment")]
    public class OtherInvestmentController : BaseController
    {
        private const int OkResult = 200;
        private readonly IOtherInvestmentService otherInvestmentService;
        private readonly IOtherInvestmentBalaceHistoryService otherInvestmentBalaceHistoryService;
        private readonly IOtherInvestmentTagService otherInvestmentTagService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherInvestmentController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="otherInvestmentService">Service for other investment-related operations.</param>
        /// <param name="otherInvestmentBalaceHistoryService">Service for investment balance history operations.</param>
        /// <param name="otherInvestmentTagService">Service for investment tag-related operations.</param>
        public OtherInvestmentController(IHttpContextAccessor httpContextAccessor, IOtherInvestmentService otherInvestmentService,
            IOtherInvestmentBalaceHistoryService otherInvestmentBalaceHistoryService, IOtherInvestmentTagService otherInvestmentTagService) : base(httpContextAccessor)
        {
            this.otherInvestmentService = otherInvestmentService;
            this.otherInvestmentBalaceHistoryService = otherInvestmentBalaceHistoryService;
            this.otherInvestmentTagService = otherInvestmentTagService;
        }

        /// <summary>
        /// Retrieves all investments for the current user.
        /// </summary>
        /// <returns>A collection of other investment models.</returns>
        /// <response code="200">Returns the investment data successfully.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<OtherInvestmentModel>> Get()
        {
            return Ok(otherInvestmentService.GetAll(GetUserId()));
        }

        /// <summary>
        /// Adds a new investment to the system.
        /// </summary>
        /// <param name="otherInvestment">The investment model containing the investment details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The investment was added successfully.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public IActionResult Add([FromBody] OtherInvestmentModel otherInvestment)
        {
            otherInvestment.UserIdentityId = GetUserId();
            otherInvestmentService.Add(otherInvestment);
            return Ok();
        }

        /// <summary>
        /// Updates an existing investment.
        /// </summary>
        /// <param name="otherInvestment">The investment model containing the updated investment details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The investment was updated successfully.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public IActionResult Update([FromBody] OtherInvestmentModel otherInvestment)
        {
            otherInvestment.UserIdentityId = GetUserId();
            otherInvestmentService.Update(otherInvestment);
            return Ok();
        }

        /// <summary>
        /// Deletes an investment from the system.
        /// </summary>
        /// <param name="id">The ID of the investment to delete.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The investment was deleted successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete]
        public IActionResult Delete([FromBody] int id)
        {
            if (!otherInvestmentService.UserHasRightToPayment(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            otherInvestmentService.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Retrieves balance history for a specific investment.
        /// </summary>
        /// <param name="otherInvestmentId">The ID of the investment to retrieve balance history for.</param>
        /// <returns>A collection of balance history models for the specified investment.</returns>
        /// <response code="200">Returns the balance history successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{otherInvestmentId}/balanceHistory")]
        public ActionResult<IEnumerable<OtherInvestmentBalaceHistoryModel>> Get(int otherInvestmentId)
        {
            if (CheckUserRigth(otherInvestmentId) is var result && result.StatusCode != OkResult)
                return result;

            return Ok(otherInvestmentBalaceHistoryService.Get(c => c.OtherInvestmentId == otherInvestmentId));
        }

        /// <summary>
        /// Adds a balance history entry to a specific investment.
        /// </summary>
        /// <param name="otherInvestmentId">The ID of the investment to add balance history to.</param>
        /// <param name="otherInvestmentBalaceHistory">The balance history model to add.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The balance history entry was added successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("{otherInvestmentId}/balanceHistory")]
        public IActionResult AddHistoryBalance(int otherInvestmentId, [FromBody] OtherInvestmentBalaceHistoryModel otherInvestmentBalaceHistory)
        {
            if (CheckUserRigth(otherInvestmentId) is var result && result.StatusCode != OkResult)
                return result;

            otherInvestmentBalaceHistory.OtherInvestmentId = otherInvestmentId;
            otherInvestmentBalaceHistoryService.Add(otherInvestmentBalaceHistory);
            return Ok();
        }

        /// <summary>
        /// Retrieves all balance history entries for the current user's investments.
        /// </summary>
        /// <returns>A collection of all balance history entries for the user's investments.</returns>
        /// <response code="200">Returns the balance history entries successfully.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("otherInvestment/balance")]
        public ActionResult GetAllBalances() => Ok(otherInvestmentBalaceHistoryService.Get(b => b.OtherInvestment.UserIdentityId == GetUserId()));

        /// <summary>
        /// Updates an existing balance history entry.
        /// </summary>
        /// <param name="otherInvestmentBalaceHistory">The balance history model containing the updated details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The balance history entry was updated successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("/balanceHistory")]
        public IActionResult UpdateHistoryBalance([FromBody] OtherInvestmentBalaceHistoryModel otherInvestmentBalaceHistory)
        {
            if (CheckUserRigth(otherInvestmentBalaceHistory.OtherInvestmentId) is var result && result.StatusCode != OkResult)
                return result;

            otherInvestmentBalaceHistoryService.Update(otherInvestmentBalaceHistory);
            return Ok();
        }

        /// <summary>
        /// Deletes a balance history entry from the system.
        /// </summary>
        /// <param name="id">The ID of the balance history entry to delete.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The balance history entry was deleted successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("/balanceHistory")]
        public IActionResult DeleteHistoryBalance([FromBody] int id)
        {
            if (CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            otherInvestmentService.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Calculates the profit over a specified number of years for an investment.
        /// </summary>
        /// <param name="id">The ID of the investment to calculate profit for.</param>
        /// <param name="years">Optional. The number of years to calculate profit for. If null, calculates for all available years.</param>
        /// <returns>The calculated profit amount.</returns>
        /// <response code="200">Returns the profit calculation successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{id}/profitOverYears/{years}")]
        public async Task<ActionResult<decimal>> ProfitOverYears(int id, int? years = null)
        {
            if (CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            decimal profit = await otherInvestmentService.GetProgressForYears(id, years);
            return Ok(profit);
        }

        /// <summary>
        /// Calculates the overall profit for an investment since inception.
        /// </summary>
        /// <param name="id">The ID of the investment to calculate overall profit for.</param>
        /// <returns>The calculated overall profit amount.</returns>
        /// <response code="200">Returns the overall profit calculation successfully.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}/profitOverall")]
        public async Task<ActionResult<decimal>> ProfitOverall(int id)
            => await ProfitOverYears(id);

        /// <summary>
        /// Retrieves all payments associated with a specific tag for an investment.
        /// </summary>
        /// <param name="id">The ID of the investment.</param>
        /// <param name="tagId">The ID of the tag to filter payments by.</param>
        /// <returns>A collection of payment models associated with the specified tag and investment.</returns>
        /// <response code="200">Returns the tagged payments successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{id}/tagedPayments/{tagId}")]
        public async Task<ActionResult<IEnumerable<PaymentModel>>> GetTagedPayments(int id, int tagId)
        {
            if (CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            return Ok(await otherInvestmentTagService.GetPaymentsForTag(id, tagId));
        }

        /// <summary>
        /// Retrieves the tag linked to a specific investment.
        /// </summary>
        /// <param name="id">The ID of the investment to get the linked tag for.</param>
        /// <returns>The investment tag model linked to the specified investment.</returns>
        /// <response code="200">Returns the linked tag successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{id}/linkedTag")]
        public ActionResult<OtherInvestmentTagModel> GetLinkedTag(int id)
        {
            if (CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            return Ok(otherInvestmentTagService.Get(c => c.OtherInvestmentId == id).SingleOrDefault());
        }

        /// <summary>
        /// Links an investment with a specific tag.
        /// </summary>
        /// <param name="id">The ID of the investment to link.</param>
        /// <param name="tagId">The ID of the tag to link with the investment.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The investment was linked with the tag successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified investment.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("{id}/tagedPayments/{tagId}")]
        public IActionResult LinkInvestmentWithTag(int id, int tagId)
        {
            if (CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            otherInvestmentTagService.ReplaceTagForOtherInvestment(id, tagId);
            return Ok();
        }

        /// <summary>
        /// Retrieves a summary of all investments for the current user.
        /// </summary>
        /// <returns>A summary model containing aggregated information about the user's investments.</returns>
        /// <response code="200">Returns the investment summary successfully.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("summary")]
        public ActionResult<OtherInvestmentBalanceSummaryModel> GetOtherInvestmentSummary()
            => Ok(otherInvestmentService.GetAllInvestmentSummary(GetUserId()));

        /// <summary>
        /// Checks if the current user has rights to access the specified investment.
        /// </summary>
        /// <param name="otherInvestmentId">The ID of the investment to check rights for.</param>
        /// <returns>A StatusCodeResult indicating whether the user has rights to the investment.</returns>
        private StatusCodeResult CheckUserRigth(int otherInvestmentId)
        {
            if (!otherInvestmentService.UserHasRightToPayment(otherInvestmentId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok();
        }
    }
}