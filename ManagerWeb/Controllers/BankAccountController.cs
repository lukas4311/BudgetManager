using System;
using System.Collections.Generic;
using System.Linq;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace ManagerWeb.Controllers
{
    public partial class BankAccountController : Controller
    {
        private readonly IBankAccountService bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            this.bankAccountService = bankAccountService;
        }

        [HttpGet]
        public JsonResult GetBankAccountsBalanceToDate(DateTime? toDate)
        {
            IEnumerable<BankBalanceModel> bankInfo = this.bankAccountService.GetBankAccountsBalanceToDate(toDate);

            return Json(new { success = true, bankAccountsBalance = bankInfo });
        }
    }
}