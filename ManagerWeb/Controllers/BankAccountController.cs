using System;
using System.Collections.Generic;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagerWeb.Controllers
{
    [Route("bankAccount")]
    public partial class BankAccountController : Controller
    {
        private readonly IBankAccountService bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            this.bankAccountService = bankAccountService;
        }

        [HttpGet("getAllAccountBalance")]
        public JsonResult GetBankAccountsBalanceToDate([FromQuery] DateTime? toDate = null)
        {
            IEnumerable<BankBalanceModel> bankInfo = this.bankAccountService.GetBankAccountsBalanceToDate(toDate);

            return Json(new { success = true, bankAccountsBalance = bankInfo });
        }
    }
}