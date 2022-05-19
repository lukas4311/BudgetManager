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

        [HttpGet("{otherInvestmentId}/balanceHistory")]
        public ActionResult<IEnumerable<OtherInvestmentBalaceHistoryModel>> Get(int otherInvestmentId)
        {
            if (this.CheckUserRigth(otherInvestmentId) is var result && result.StatusCode != OkResult)
                return result;

            return Ok(this.otherInvestmentBalaceHistoryService.Get(c => c.OtherInvestmentId == otherInvestmentId));
        }

        [HttpPost("{otherInvestmentId}/balanceHistory")]
        public IActionResult AddHistoryBalance(int otherInvestmentId, [FromBody] OtherInvestmentBalaceHistoryModel otherInvestmentBalaceHistory)
        {
            if (this.CheckUserRigth(otherInvestmentId) is var result && result.StatusCode != OkResult)
                return result;

            otherInvestmentBalaceHistory.OtherInvestmentId = otherInvestmentId;
            this.otherInvestmentBalaceHistoryService.Add(otherInvestmentBalaceHistory);
            return Ok();
        }

        [HttpGet("otherInvestment/balance")]
        public ActionResult GetAllBalances()
        {
            // TODO: get all my other investments with costs and balances
            return Ok();
        }

        [HttpPut("/balanceHistory")]
        public IActionResult UpdateHistoryBalance([FromBody] OtherInvestmentBalaceHistoryModel otherInvestmentBalaceHistory)
        {
            if (this.CheckUserRigth(otherInvestmentBalaceHistory.OtherInvestmentId) is var result && result.StatusCode != OkResult)
                return result;

            this.otherInvestmentBalaceHistoryService.Update(otherInvestmentBalaceHistory);
            return Ok();
        }

        [HttpDelete("/balanceHistory")]
        public IActionResult DeleteHistoryBalance([FromBody] int id)
        {
            if (this.CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            this.otherInvestmentService.Delete(id);
            return Ok();
        }

        [HttpGet("{id}/profitOverYears/{years}")]
        public async Task<ActionResult<decimal>> ProfitOverYears(int id, int? years = null)
        {
            if (this.CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            decimal profit = await this.otherInvestmentService.GetProgressForYears(id, years);
            return Ok(profit);
        }

        [HttpGet("{id}/profitOverall")]
        public async Task<ActionResult<decimal>> ProfitOverall(int id)
            => await this.ProfitOverYears(id);

        [HttpGet("{id}/tagedPayments/{tagId}")]
        public async Task<ActionResult<IEnumerable<PaymentModel>>> GetTagedPayments(int id, int tagId)
        {
            if (this.CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            return Ok(await this.otherInvestmentTagService.GetPaymentsForTag(id, tagId));
        }

        [HttpGet("{id}/linkedTag")]
        public ActionResult<OtherInvestmentTagModel> GetLinkedTag(int id)
        {
            if (this.CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            return this.otherInvestmentTagService.Get(c => c.OtherInvestmentId == id).SingleOrDefault();
        }

        [HttpPost("{id}/tagedPayments/{tagId}")]
        public IActionResult LinkInvestmentWithTag(int id, int tagId)
        {
            if (this.CheckUserRigth(id) is var result && result.StatusCode != OkResult)
                return result;

            this.otherInvestmentTagService.ReplaceTagForOtherInvestment(id,tagId);
            return Ok();
        }

        [HttpGet("summary")]
        public ActionResult<OtherInvestmentBalanceSummaryModel> GetOtherInvestmentSummary() 
            => Ok(this.otherInvestmentService.GetAllInvestmentSummary(this.GetUserId()));

        private StatusCodeResult CheckUserRigth(int otherInvestmentId)
        {
            if (!this.otherInvestmentService.UserHasRightToPayment(otherInvestmentId, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok();
        }
    }
}
