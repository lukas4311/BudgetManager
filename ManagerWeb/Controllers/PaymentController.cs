using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.DataModels;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace ManagerWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentTypeRepository paymentTypeRepository;
        private readonly IPaymentCategoryRepository paymentCategoryRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IBankAccountRepository bankAccountRepository;

        public PaymentController(IPaymentTypeRepository paymentTypeRepository, IPaymentCategoryRepository paymentCategoryRepository, IPaymentRepository paymentRepository, IBankAccountRepository bankAccountRepository)
        {
            this.paymentTypeRepository = paymentTypeRepository;
            this.paymentCategoryRepository = paymentCategoryRepository;
            this.paymentRepository = paymentRepository;
            this.bankAccountRepository = bankAccountRepository;
        }

        [HttpGet]
        public JsonResult GetPaymentsData(DateTime? fromDate)
        {
            List<PaymentViewModel> payments = this.paymentRepository.FindAll().Where(a => a.Date > (fromDate ?? DateTime.MinValue)).Select(a => new PaymentViewModel {
                Amount = a.Amount,
                Date = a.Date,
                Id = a.Id,
                Name = a.Name,
                Description = a.Description
            }).ToList();

            return Json(payments);
        }

        [HttpGet]
        public JsonResult GetPaymentTypes()
        {
            List<PaymentTypeModel> paymentTypes = this.paymentTypeRepository.FindAll().Select(p => new PaymentTypeModel
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            return Json(new { success = true, types = paymentTypes });
        }

        [HttpGet]
        public JsonResult GetPaymentCategories()
        {
            List<PaymentCategoryModel> paymentCategories = this.paymentCategoryRepository.FindAll().Select(p => new PaymentCategoryModel
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            return Json(new { success = true, categories = paymentCategories });
        }

        [HttpGet]
        public JsonResult GetBankAccounts()
        {
            List<BankAccount> bankAccounts = this.bankAccountRepository.FindAll()
                //.Include(b => b.UserDataId == loggedUserId)
                .ToList();

            return Json(new { success = true, bankAccounts });
        }

        [HttpPost]
        public JsonResult AddPayment([FromBody] PaymentViewModel paymentViewModel)
        {
            Payment payment = new Payment
            {
                Amount = paymentViewModel.Amount,
                Date = paymentViewModel.Date,
                Description = paymentViewModel.Description,
                Id = paymentViewModel.Id,
                Name = paymentViewModel.Name,
                PaymentCategoryId = paymentViewModel.PaymentCategoryId.Value,
                PaymentTypeId = paymentViewModel.PaymentTypeId.Value,
                BankAccountId = paymentViewModel.BankAccountId.Value
            };

            this.paymentRepository.Create(payment);
            this.paymentRepository.Save();

            return Json(new { success = true });
        }
    }
}