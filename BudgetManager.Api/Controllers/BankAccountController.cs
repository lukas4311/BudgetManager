﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
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

        [HttpPost("add")]
        public IActionResult AddBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            int paymentId = this.bankAccountService.AddBankAccount(bankAccountViewModel);
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult UpdateBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            this.bankAccountService.UpdateBankAccount(bankAccountViewModel);
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult DeleteBankAccount([FromBody] int id)
        {
            this.bankAccountService.DeleteBankAccount(id);
            return this.Ok();
        }
    }
}