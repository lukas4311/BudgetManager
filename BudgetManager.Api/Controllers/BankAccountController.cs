using System;
using System.Collections.Generic;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing bank account operations in the Budget Manager application.
    /// Provides endpoints for retrieving, creating, updating, and deleting bank accounts,
    /// as well as fetching account balances.
    /// </summary>
    [ApiController]
    [Route("bankAccounts")]
    public partial class BankAccountController : BaseController
    {
        private readonly IBankAccountService bankAccountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BankAccountController"/> class.
        /// </summary>
        /// <param name="bankAccountService">The service that handles bank account operations.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        public BankAccountController(IBankAccountService bankAccountService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this.bankAccountService = bankAccountService;
        }

        /// <summary>
        /// Retrieves the balance of all bank accounts belonging to the current user up to a specified date.
        /// </summary>
        /// <param name="toDate">Optional. The date up to which to calculate the balance. If not provided, the current date is used.</param>
        /// <returns>A collection of bank account balances.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all/balance/{toDate}")]
        public ActionResult<IEnumerable<BankBalanceModel>> GetUserBankAccountsBalanceToDate(DateTime? toDate = null)
        {
            return Ok(bankAccountService.GetBankAccountsBalanceToDate(GetUserId(), toDate));
        }

        /// <summary>
        /// Retrieves the balance of a specific bank account up to a specified date.
        /// </summary>
        /// <param name="bankAccountId">The ID of the bank account.</param>
        /// <param name="toDate">Optional. The date up to which to calculate the balance. If not provided, the current date is used.</param>
        /// <returns>The bank account balance or an unauthorized status code if the user doesn't have access to the specified account.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{bankAccountId}/balance/{toDate}")]
        public ActionResult<BankBalanceModel> GetBalance(int bankAccountId, DateTime? toDate = null)
        {
            if (!bankAccountService.UserHasRightToBankAccount(bankAccountId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);
            return Ok(bankAccountService.GetBankAccountBalanceToDate(bankAccountId, toDate));
        }

        /// <summary>
        /// Retrieves all bank accounts belonging to the current user.
        /// </summary>
        /// <returns>A collection of bank accounts.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("all")]
        public ActionResult<IEnumerable<BankAccountModel>> All()
        {
            return Ok(bankAccountService.GetAllBankAccounts(GetUserId()));
        }

        /// <summary>
        /// Creates a new bank account for the current user.
        /// </summary>
        /// <param name="bankAccountViewModel">The bank account data.</param>
        /// <returns>The ID of the newly created bank account.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public IActionResult AddBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            bankAccountViewModel.UserIdentityId = GetUserId();
            int paymentId = bankAccountService.Add(bankAccountViewModel);
            return Ok(paymentId);
        }

        /// <summary>
        /// Updates an existing bank account.
        /// </summary>
        /// <param name="bankAccountViewModel">The updated bank account data.</param>
        /// <returns>A success status code.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPut]
        public IActionResult UpdateBankAccount([FromBody] BankAccountModel bankAccountViewModel)
        {
            bankAccountViewModel.UserIdentityId = GetUserId();
            bankAccountService.Update(bankAccountViewModel);
            return Ok();
        }

        /// <summary>
        /// Deletes a bank account.
        /// </summary>
        /// <param name="bankAccountId">The ID of the bank account to delete.</param>
        /// <returns>A success status code or an unauthorized status code if the user doesn't have access to the specified account.</returns>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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