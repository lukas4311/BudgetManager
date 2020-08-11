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
    public class BankAccountController : Controller
    {
        private readonly IPaymentTypeRepository paymentTypeRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly IBankAccountRepository bankAccountRepository;

        public BankAccountController(IPaymentTypeRepository paymentTypeRepository, IPaymentRepository paymentRepository, IBankAccountRepository bankAccountRepository)
        {
            this.paymentTypeRepository = paymentTypeRepository;
            this.paymentRepository = paymentRepository;
            this.bankAccountRepository = bankAccountRepository;
        }

        [HttpGet]
        public IActionResult GetBankAccountBalanceToDate(DateTime? toDate)
        {
            return null;
        }
    }
}