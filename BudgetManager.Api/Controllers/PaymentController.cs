using BudgetManager.Data.DataModels;
using System.Collections.Generic;
using System;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BudgetManager.Domain.DTOs;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("payment")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly ITagService tagService;

        public PaymentController(IHttpContextAccessor httpContextAccessor, IPaymentService paymentService, ITagService tagService) : base(httpContextAccessor)
        {
            this.paymentService = paymentService;
            this.tagService = tagService;
        }

        [HttpGet("data")]
        public ActionResult<IEnumerable<PaymentModel>> GetPaymentsData([FromQuery] DateTime? fromDate, DateTime? toDate, int? bankAccountId = null)
        {
            IEnumerable<PaymentModel> payments = this.paymentService.GetPaymentsData(fromDate, toDate, this.GetUserId() ,bankAccountId);
            return Ok(payments);
        }

        [HttpGet("types")]
        public ActionResult<IEnumerable<PaymentTypeModel>> GetPaymentTypes()
        {
            return this.paymentService.GetPaymentTypes();
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<PaymentCategoryModel>> GetPaymentCategories()
        {
            return this.paymentService.GetPaymentCategories();
        }

        [HttpGet("bankAccounts")]
        public IActionResult GetBankAccounts()
        {
            IEnumerable<BankAccountModel> bankAccounts = this.paymentService.GetBankAccounts();
            return Ok(new { success = true, bankAccounts });
        }

        [HttpPost]
        public IActionResult AddPayment([FromBody] PaymentModel paymentViewModel)
        {
            int paymentId = this.paymentService.Add(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentId);
            return Ok(new { success = true });
        }

        [HttpPut]
        public IActionResult UpdatePayment([FromBody] PaymentModel paymentViewModel)
        {
            this.paymentService.Update(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentViewModel.Id.Value);
            return Ok(new { success = true });
        }

        [HttpGet("detail")]
        public ActionResult<PaymentModel> GetPayment([FromQuery] int id)
        {
            PaymentModel payment = this.paymentService.Get(id);
            return Ok(payment);
        }

        [HttpPost("clone")]
        public IActionResult ClonePayment([FromBody] PaymentModel paymentViewModel)
        {
            this.paymentService.ClonePayment(paymentViewModel.Id.Value);
            return Ok();
        }
    }
}
