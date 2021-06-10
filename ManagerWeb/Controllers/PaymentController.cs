using System;
using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.ManagerWeb.Models.DTOs;
using BudgetManager.ManagerWeb.Models.ViewModels;
using BudgetManager.ManagerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.ManagerWeb.Controllers
{
    [ApiController]
    [Authorize]
    [Route("payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly ITagService tagService;

        public PaymentController(IPaymentService paymentService, ITagService tagService)
        {
            this.paymentService = paymentService;
            this.tagService = tagService;
        }

        [HttpGet("data")]
        public ActionResult<IEnumerable<PaymentViewModel>> GetPaymentsData([FromQuery] DateTime? fromDate, DateTime? toDate, int? bankAccountId = null)
        {
            IEnumerable<PaymentViewModel> payments = this.paymentService.GetPaymentsData(fromDate, toDate, bankAccountId);
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
            IEnumerable<BankAccount> bankAccounts = this.paymentService.GetBankAccounts();
            return Ok(new { success = true, bankAccounts });
        }

        [HttpPost]
        public IActionResult AddPayment([FromBody] PaymentViewModel paymentViewModel)
        {
            int paymentId = this.paymentService.AddPayment(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentId);
            return Ok(new { success = true });
        }

        [HttpPut]
        public IActionResult UpdatePayment([FromBody] PaymentViewModel paymentViewModel)
        {
            this.paymentService.UpdatePayment(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentViewModel.Id.Value);
            return Ok(new { success = true });
        }

        [HttpGet("detail")]
        public ActionResult<PaymentViewModel> GetPayment([FromQuery] int id)
        {
            PaymentViewModel payment = this.paymentService.GetPayment(id);
            return Ok(payment);
        }
    }
}