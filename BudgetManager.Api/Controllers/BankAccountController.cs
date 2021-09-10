using System;
using System.Collections.Generic;
using System.Security.Claims;
using BudgetManager.Api.Extensions;
using BudgetManager.Api.Models;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("bankAccount")]
    public partial class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BankAccountController(IBankAccountService bankAccountService, IHttpContextAccessor httpContextAccessor)
        {
            this.bankAccountService = bankAccountService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("allAccountBalance/{toDate}")]
        public ActionResult<IEnumerable<BankBalanceModel>> GetUserBankAccountsBalanceToDate(DateTime? toDate = null)
        {
            return Ok(this.bankAccountService.GetBankAccountsBalanceToDate(this.httpContextAccessor.HttpContext.GetUserId(), toDate));
        }

        [HttpGet("balance/{bankAccountId}/{toDate}")]
        public ActionResult<BankBalanceModel> GetBalance(int bankAccountId, DateTime? toDate = null)
        {
            return Ok(this.bankAccountService.GetBankAccountBalanceToDate(bankAccountId, toDate));
        }

        [HttpGet("allAccounts/{userId}")]
        public ActionResult<IEnumerable<BankAccountModel>> All(int userId)
        {
            return Ok(this.bankAccountService.GetAllBankAccounts(userId));
        }

        [HttpPost]
        public IActionResult AddBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            int paymentId = this.bankAccountService.Add(bankAccountViewModel);
            return Ok(paymentId);
        }

        [HttpPut]
        public IActionResult UpdateBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            this.bankAccountService.Update(bankAccountViewModel);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteBankAccount([FromBody] int id)
        {
            this.bankAccountService.Delete(id);
            return this.Ok();
        }
    }
}
