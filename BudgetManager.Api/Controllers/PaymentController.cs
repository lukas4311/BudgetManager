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

        [HttpGet]
        public ActionResult<IEnumerable<PaymentModel>> GetPaymentsData([FromQuery] DateTime? fromDate, DateTime? toDate, int? bankAccountId = null)
        {
            if (bankAccountId.HasValue && !this.bankAccountService.UserHasRightToBankAccount(bankAccountId.Value, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            IEnumerable<PaymentModel> payments = this.paymentService.GetPaymentsData(fromDate, toDate, this.GetUserId(), bankAccountId);
            return Ok(payments);
        }

        [HttpGet("types")]
        public ActionResult<IEnumerable<PaymentTypeModel>> GetPaymentTypes() => this.paymentService.GetPaymentTypes();

        [HttpGet("categories")]
        public ActionResult<IEnumerable<PaymentCategoryModel>> GetPaymentCategories() => this.paymentService.GetPaymentCategories();

        [HttpPost]
        public IActionResult AddPayment([FromBody] PaymentModel paymentViewModel)
        {
            if (!this.bankAccountService.UserHasRightToBankAccount(paymentViewModel.BankAccountId.Value, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            int paymentId = this.paymentService.Add(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentId);
            return Ok();
        }

        [HttpPut]
        public IActionResult UpdatePayment([FromBody] PaymentModel paymentViewModel)
        {
            if (!this.bankAccountService.UserHasRightToBankAccount(paymentViewModel.BankAccountId.Value, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.paymentService.Update(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentViewModel.Id.Value);
            return Ok();
        }

        [HttpGet("detail")]
        public ActionResult<PaymentModel> GetPayment(int id)
        {
            PaymentModel payment = this.paymentService.Get(id);

            if (!this.bankAccountService.UserHasRightToBankAccount(payment.BankAccountId.Value, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(payment);
        }

        [HttpPost("clone")]
        public IActionResult ClonePayment([FromBody] PaymentModel paymentViewModel)
        {
            if (!this.bankAccountService.UserHasRightToBankAccount(paymentViewModel.BankAccountId.Value, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.paymentService.ClonePayment(paymentViewModel.Id.Value);
            return Ok();
        }

        [HttpDelete]
        [Route("{paymentId}/tag/{tagId}")]
        public IActionResult RemoveTagFromPayment([FromBody] int tagId, int paymentId)
        {
            if (this.paymentService.UserHasRightToPayment(paymentId, this.GetUserId()))
                return this.StatusCode(StatusCodes.Status401Unauthorized);

            this.tagService.RemoveTagFromPayment(tagId, paymentId);
            return Ok();
        }
    }
}
