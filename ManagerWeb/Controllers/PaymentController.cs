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
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService paymentService;
        private readonly ITagService tagService;

        public PaymentController(IPaymentService paymentService, ITagService tagService)
        {
            this.paymentService = paymentService;
            this.tagService = tagService;
        }

        [HttpGet]
        public JsonResult GetPaymentsData(DateTime? fromDate, int? bankAccountId)
        {
            List<PaymentViewModel> payments = this.paymentService.GetPaymentsData(fromDate, bankAccountId);
            return Json(payments);
        }

        [HttpGet]
        public JsonResult GetPaymentTypes()
        {
            List<PaymentTypeModel> paymentTypes = this.paymentService.GetPaymentTypes();
            return Json(new { success = true, types = paymentTypes });
        }

        [HttpGet]
        public JsonResult GetPaymentCategories()
        {
            List<PaymentCategoryModel> paymentCategories = this.paymentService.GetPaymentCategories();
            return Json(new { success = true, categories = paymentCategories });
        }

        [HttpGet]
        public JsonResult GetBankAccounts()
        {
            List<BankAccount> bankAccounts = this.paymentService.GetBankAccounts();
            return Json(new { success = true, bankAccounts });
        }

        [HttpPost]
        public JsonResult AddPayment([FromBody] PaymentViewModel paymentViewModel)
        {
            int paymentId = this.paymentService.AddPayment(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentId);
            return Json(new { success = true });
        }

        [HttpPut]
        public JsonResult UpdatePayment([FromBody] PaymentViewModel paymentViewModel)
        {
            this.paymentService.UpdatePayment(paymentViewModel);
            this.tagService.UpdateAllTags(paymentViewModel.Tags, paymentViewModel.Id.Value);
            return Json(new { success = true });
        }

        [HttpGet]
        public JsonResult GetPayment(int id)
        {
            PaymentViewModel payment = this.paymentService.GetPayment(id);
            return Json(new { success = true, payment });
        }
    }
}