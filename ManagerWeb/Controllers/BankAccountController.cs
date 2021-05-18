using System;
using System.Collections.Generic;
using ManagerWeb.Models.DTOs;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManagerWeb.Controllers
{
    [ApiController]
    [Route("bankAccount")]
    public partial class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            this.bankAccountService = bankAccountService;
        }

        [HttpGet("getAllAccountBalance")]
        public ActionResult<IEnumerable<BankBalanceModel>> GetBankAccountsBalanceToDate([FromQuery] DateTime? toDate = null)
        {
            IEnumerable<BankBalanceModel> bankInfo = this.bankAccountService.GetBankAccountsBalanceToDate(toDate);
            return Ok(new { success = true, bankAccountsBalance = bankInfo });
        }

        [HttpGet("getAll")]
        public ActionResult<IEnumerable<BankAccountModel>> GetAll()
        {
            return Ok(this.bankAccountService.GetAllBankAccounts());
        }
    }
}