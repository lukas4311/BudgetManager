using System;
using System.Collections.Generic;
using Data.DataModels;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Models.ViewModels;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagerWeb.Controllers
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
        public IActionResult GetPaymentsData([FromQuery] DateTime? fromDate, DateTime? toDate, int? bankAccountId = null)
        {
            List<PaymentViewModel> payments = this.paymentService.GetPaymentsData(fromDate, toDate, bankAccountId);
            return Ok(payments);
        }

        [HttpGet("types")]
        public IActionResult GetPaymentTypes()
        {
            List<PaymentTypeModel> paymentTypes = this.paymentService.GetPaymentTypes();
            return Ok(new { success = true, types = paymentTypes });
        }

        [HttpGet("categories")]
        public IActionResult GetPaymentCategories()
        {
            List<PaymentCategoryModel> paymentCategories = this.paymentService.GetPaymentCategories();
            return Ok(new { success = true, categories = paymentCategories });
        }

        [HttpGet("bankAccounts")]
        public IActionResult GetBankAccounts()
        {
            List<BankAccount> bankAccounts = this.paymentService.GetBankAccounts();
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
        public IActionResult GetPayment([FromQuery] int id)
        {
            PaymentViewModel payment = this.paymentService.GetPayment(id);
            return Ok(new { success = true, payment });
        }
    }
}