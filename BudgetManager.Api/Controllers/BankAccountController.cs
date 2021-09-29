using System;
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

        public BankAccountController(IBankAccountService bankAccountService, IHttpContextAccessor httpContextAccessor) : base (httpContextAccessor)
        {
            this.bankAccountService = bankAccountService;
        }

        [HttpGet("all/balance/{toDate}")]
        public ActionResult<IEnumerable<BankBalanceModel>> GetUserBankAccountsBalanceToDate(DateTime? toDate = null)
        {
            return Ok(this.bankAccountService.GetBankAccountsBalanceToDate(this.GetUserId(), toDate));
        }

        [HttpGet("{bankAccountId}/balance/{toDate}")]
        public ActionResult<BankBalanceModel> GetBalance(int bankAccountId, DateTime? toDate = null)
        {
            if (!this.bankAccountService.UserHasRightToBankAccount(bankAccountId, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(this.bankAccountService.GetBankAccountBalanceToDate(bankAccountId, toDate));
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<BankAccountModel>> All()
        {
            return Ok(this.bankAccountService.GetAllBankAccounts(this.GetUserId()));
        }

        [HttpPost]
        public IActionResult AddBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            if (bankAccountViewModel.UserIdentityId != this.GetUserId())
                return StatusCode(StatusCodes.Status401Unauthorized);

            int paymentId = this.bankAccountService.Add(bankAccountViewModel);
            return Ok(paymentId);
        }

        [HttpPut]
        public IActionResult UpdateBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            if (bankAccountViewModel.UserIdentityId != this.GetUserId())
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.bankAccountService.Update(bankAccountViewModel);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteBankAccount([FromBody] int bankAccountId)
        {
            if(!this.bankAccountService.UserHasRightToBankAccount(bankAccountId, this.GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            this.bankAccountService.Delete(bankAccountId);
            return this.Ok();
        }
    }
}
