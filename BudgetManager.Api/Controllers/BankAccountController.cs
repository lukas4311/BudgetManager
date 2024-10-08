﻿using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("bankAccounts")]
    public partial class BankAccountController : BaseController
    {
        private readonly IBankAccountService bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this.bankAccountService = bankAccountService;
        }

        [HttpGet("all/balance/{toDate}")]
        public ActionResult<IEnumerable<BankBalanceModel>> GetUserBankAccountsBalanceToDate(DateTime? toDate = null)
        {
            return Ok(bankAccountService.GetBankAccountsBalanceToDate(GetUserId(), toDate));
        }

        [HttpGet("{bankAccountId}/balance/{toDate}")]
        public ActionResult<BankBalanceModel> GetBalance(int bankAccountId, DateTime? toDate = null)
        {
            if (!bankAccountService.UserHasRightToBankAccount(bankAccountId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(bankAccountService.GetBankAccountBalanceToDate(bankAccountId, toDate));
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<BankAccountModel>> All()
        {
            return Ok(bankAccountService.GetAllBankAccounts(GetUserId()));
        }

        [HttpPost]
        public IActionResult AddBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            bankAccountViewModel.UserIdentityId = GetUserId();
            int paymentId = bankAccountService.Add(bankAccountViewModel);
            return Ok(paymentId);
        }

        [HttpPut]
        public IActionResult UpdateBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            bankAccountViewModel.UserIdentityId = GetUserId();
            bankAccountService.Update(bankAccountViewModel);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteBankAccount([FromBody] int bankAccountId)
        {
            if (!bankAccountService.UserHasRightToBankAccount(bankAccountId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            bankAccountService.Delete(bankAccountId);
            return Ok();
        }
    }
}
