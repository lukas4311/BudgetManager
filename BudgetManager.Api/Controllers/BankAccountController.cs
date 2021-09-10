using System;
using System.Collections.Generic;
using BudgetManager.Api.Extensions;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("bankAccount")]
    public partial class BankAccountController : BaseController
    {
        private readonly IBankAccountService bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService, IHttpContextAccessor httpContextAccessor) : base (httpContextAccessor)
        {
            this.bankAccountService = bankAccountService;
        }

        [HttpGet("allAccountBalance/{toDate}")]
        public ActionResult<IEnumerable<BankBalanceModel>> GetUserBankAccountsBalanceToDate(DateTime? toDate = null)
        {
            return Ok(this.bankAccountService.GetBankAccountsBalanceToDate(this.GetUserId(), toDate));
        }

        [HttpGet("balance/{bankAccountId}/{toDate}")]
        public ActionResult<BankBalanceModel> GetBalance(int bankAccountId, DateTime? toDate = null)
        {
            return Ok(this.bankAccountService.GetBankAccountBalanceToDate(bankAccountId, toDate));
        }

        [HttpGet("allAccounts/{userId}")]
        public ActionResult<IEnumerable<BankAccountModel>> All()
        {
            return Ok(this.bankAccountService.GetAllBankAccounts(this.GetUserId()));
        }

        [HttpPost]
        public IActionResult AddBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            if (bankAccountViewModel.UserIdentityId != this.GetUserId())
                return StatusCode(401);

            int paymentId = this.bankAccountService.Add(bankAccountViewModel);
            return Ok(paymentId);
        }

        [HttpPut]
        public IActionResult UpdateBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            if (bankAccountViewModel.UserIdentityId != this.GetUserId())
                return StatusCode(401);

            this.bankAccountService.Update(bankAccountViewModel);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteBankAccount([FromBody] int id)
        {
            if(!this.bankAccountService.UserHasRightToBankAccount(id, this.GetUserId()))
                return StatusCode(401);

            this.bankAccountService.Delete(id);
            return this.Ok();
        }
    }
}
