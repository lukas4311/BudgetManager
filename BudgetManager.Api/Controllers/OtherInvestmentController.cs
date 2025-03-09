using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("otherInvestment")]
    public class OtherInvestmentController : BaseController
    {
        private const int OkResult = 200;
        private readonly IOtherInvestmentService otherInvestmentService;
        private readonly IOtherInvestmentBalaceHistoryService otherInvestmentBalaceHistoryService;
        private readonly IOtherInvestmentTagService otherInvestmentTagService;

        public OtherInvestmentController(IHttpContextAccessor httpContextAccessor, IOtherInvestmentService otherInvestmentService,
            IOtherInvestmentBalaceHistoryService otherInvestmentBalaceHistoryService, IOtherInvestmentTagService otherInvestmentTagService) : base(httpContextAccessor)
        {
            this.otherInvestmentService = otherInvestmentService;
            this.otherInvestmentBalaceHistoryService = otherInvestmentBalaceHistoryService;
            this.otherInvestmentTagService = otherInvestmentTagService;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<OtherInvestmentModel>> Get()
        {
            return Ok(otherInvestmentService.GetAll(GetUserId()));
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public IActionResult Add([FromBody] OtherInvestmentModel otherInvestment)
        {
            otherInvestment.UserIdentityId = GetUserId();
            otherInvestmentService.Add(otherInvestment);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public IActionResult Update([FromBody] OtherInvestmentModel otherInvestment)
        {
            otherInvestment.UserIdentityId = GetUserId();
            otherInvestmentService.Update(otherInvestment);
            return Ok();
        }

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

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{otherInvestmentId}/balanceHistory")]
        public ActionResult<IEnumerable<OtherInvestmentBalaceHistoryModel>> Get(int otherInvestmentId)
        {
            if (CheckUserRigth(otherInvestmentId) is var result && result.StatusCode != OkResult)
                return result;

            return Ok(otherInvestmentBalaceHistoryService.Get(c => c.OtherInvestmentId == otherInvestmentId));
        }

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

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("otherInvestment/balance")]
        public ActionResult GetAllBalances() => Ok(otherInvestmentBalaceHistoryService.Get(b => b.OtherInvestment.UserIdentityId == GetUserId()));

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

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}/profitOverall")]
        public async Task<ActionResult<decimal>> ProfitOverall(int id)
            => await ProfitOverYears(id);

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

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("{id}/tagedPayments/{tagId}")]
        public IActionResult LinkInvestmentWithTag(int id, int tagId)
        {
            if (CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            otherInvestmentTagService.ReplaceTagForOtherInvestment(id,tagId);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("summary")]
        public ActionResult<OtherInvestmentBalanceSummaryModel> GetOtherInvestmentSummary() 
            => Ok(otherInvestmentService.GetAllInvestmentSummary(GetUserId()));

        private StatusCodeResult CheckUserRigth(int otherInvestmentId)
        {
            if (!otherInvestmentService.UserHasRightToPayment(otherInvestmentId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok();
        }
    }
}
