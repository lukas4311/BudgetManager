using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly ITagService tagService;
        private readonly IBankAccountService bankAccountService;

        public PaymentController(IHttpContextAccessor httpContextAccessor, IPaymentService paymentService, ITagService tagService, IBankAccountService bankAccountService) : base(httpContextAccessor)
        {
            this.paymentService = paymentService;
            this.tagService = tagService;
            this.bankAccountService = bankAccountService;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public ActionResult<IEnumerable<PaymentModel>> GetPaymentsData([FromQuery] DateTime? fromDate, DateTime? toDate, int? bankAccountId = null)
        {
            if (bankAccountId.HasValue && !bankAccountService.UserHasRightToBankAccount(bankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            IEnumerable<PaymentModel> payments = paymentService.GetPaymentsData(fromDate, toDate, GetUserId(), bankAccountId);
            return Ok(payments);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("types")]
        public ActionResult<IEnumerable<PaymentTypeModel>> GetPaymentTypes() => paymentService.GetPaymentTypes();

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("categories")]
        public ActionResult<IEnumerable<PaymentCategoryModel>> GetPaymentCategories() => paymentService.GetPaymentCategories();

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public IActionResult AddPayment([FromBody] PaymentModel paymentViewModel)
        {
            if (!bankAccountService.UserHasRightToBankAccount(paymentViewModel.BankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            int paymentId = paymentService.Add(paymentViewModel);
            tagService.UpdateAllTags(paymentViewModel.Tags, paymentId);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut]
        public IActionResult UpdatePayment([FromBody] PaymentModel paymentViewModel)
        {
            if (!bankAccountService.UserHasRightToBankAccount(paymentViewModel.BankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            paymentService.Update(paymentViewModel);
            tagService.UpdateAllTags(paymentViewModel.Tags, paymentViewModel.Id.Value);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("detail")]
        public ActionResult<PaymentModel> GetPayment(int id)
        {
            PaymentModel payment = paymentService.Get(id);

            if (!bankAccountService.UserHasRightToBankAccount(payment.BankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(payment);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("clone/{id}")]
        public IActionResult ClonePayment(int id)
        {
            if (!paymentService.UserHasRightToPayment(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            paymentService.ClonePayment(id);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete]
        public IActionResult DeletePayment(int id)
        {
            if (!paymentService.UserHasRightToPayment(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            paymentService.Delete(id);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete]
        [Route("{paymentId}/tag/{tagId}")]
        public IActionResult RemoveTagFromPayment(int tagId, int paymentId)
        {
            if (paymentService.UserHasRightToPayment(paymentId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            tagService.RemoveTagFromPayment(tagId, paymentId);
            return Ok();
        }
    }
}
