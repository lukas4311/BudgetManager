using System;
using System.Collections.Generic;
using Asp.Versioning;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing payment operations in the Budget Manager application.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/payments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly ITagService tagService;
        private readonly IBankAccountService bankAccountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="paymentService">Service for payment-related operations.</param>
        /// <param name="tagService">Service for tag-related operations.</param>
        /// <param name="bankAccountService">Service for bank account-related operations.</param>
        public PaymentController(IHttpContextAccessor httpContextAccessor, IPaymentService paymentService, ITagService tagService, IBankAccountService bankAccountService) : base(httpContextAccessor)
        {
            this.paymentService = paymentService;
            this.tagService = tagService;
            this.bankAccountService = bankAccountService;
        }

        /// <summary>
        /// Retrieves payment data for a specified date range and bank account.
        /// </summary>
        /// <param name="fromDate">Optional start date for filtering payments.</param>
        /// <param name="toDate">Optional end date for filtering payments.</param>
        /// <param name="bankAccountId">Optional bank account ID for filtering payments.</param>
        /// <returns>A collection of payment models matching the specified criteria.</returns>
        /// <response code="200">Returns the payment data successfully.</response>
        /// <response code="401">If the user doesn't have rights to access the specified bank account.</response>
        [HttpGet, MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<PaymentModel>> GetPaymentsData([FromQuery] DateTime? fromDate, DateTime? toDate, int? bankAccountId = null)
        {
            if (bankAccountId.HasValue && !bankAccountService.UserHasRightToBankAccount(bankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            IEnumerable<PaymentModel> payments = paymentService.GetPaymentsData(fromDate, toDate, GetUserId(), bankAccountId);
            return Ok(payments);
        }

        /// <summary>
        /// Retrieves all available payment types.
        /// </summary>
        /// <returns>A collection of payment type models.</returns>
        /// <response code="200">Returns the payment types successfully.</response>
        [HttpGet("types"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<PaymentTypeModel>> GetPaymentTypes() => paymentService.GetPaymentTypes();

        /// <summary>
        /// Retrieves all available payment categories.
        /// </summary>
        /// <returns>A collection of payment category models.</returns>
        /// <response code="200">Returns the payment categories successfully.</response>
        [HttpGet("categories"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<PaymentCategoryModel>> GetPaymentCategories() => paymentService.GetPaymentCategories();

        /// <summary>
        /// Adds a new payment to the system.
        /// </summary>
        /// <param name="paymentViewModel">The payment model containing the payment details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The payment was added successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified bank account.</response>
        [HttpPost, MapToApiVersion("1.0")]
        public IActionResult AddPayment([FromBody] PaymentModel paymentViewModel)
        {
            if (!bankAccountService.UserHasRightToBankAccount(paymentViewModel.BankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            int paymentId = paymentService.Add(paymentViewModel);
            tagService.UpdateAllTags(paymentViewModel.Tags, paymentId);
            return Ok();
        }

        /// <summary>
        /// Updates an existing payment.
        /// </summary>
        /// <param name="paymentViewModel">The payment model containing the updated payment details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The payment was updated successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified bank account.</response>
        [HttpPut, MapToApiVersion("1.0")]
        public IActionResult UpdatePayment([FromBody] PaymentModel paymentViewModel)
        {
            if (!bankAccountService.UserHasRightToBankAccount(paymentViewModel.BankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            paymentService.Update(paymentViewModel);
            tagService.UpdateAllTags(paymentViewModel.Tags, paymentViewModel.Id.Value);
            return Ok();
        }

        /// <summary>
        /// Retrieves detailed information for a specific payment.
        /// </summary>
        /// <param name="id">The ID of the payment to retrieve.</param>
        /// <returns>The detailed payment model.</returns>
        /// <response code="200">Returns the payment details successfully.</response>
        /// <response code="401">If the user doesn't have rights to access the bank account associated with the payment.</response>
        [HttpGet("detail"), MapToApiVersion("1.0")]
        public ActionResult<PaymentModel> GetPayment(int id)
        {
            PaymentModel payment = paymentService.Get(id);

            if (!bankAccountService.UserHasRightToBankAccount(payment.BankAccountId.Value, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            return Ok(payment);
        }

        /// <summary>
        /// Creates a duplicate of an existing payment.
        /// </summary>
        /// <param name="id">The ID of the payment to clone.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The payment was cloned successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified payment.</response>
        [HttpPost("clone/{id}"), MapToApiVersion("1.0")]
        public IActionResult ClonePayment(int id)
        {
            if (!paymentService.UserHasRightToPayment(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            paymentService.ClonePayment(id);
            return Ok();
        }

        /// <summary>
        /// Deletes a payment from the system.
        /// </summary>
        /// <param name="id">The ID of the payment to delete.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The payment was deleted successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified payment.</response>
        [HttpDelete, MapToApiVersion("1.0")]
        public IActionResult DeletePayment(int id)
        {
            if (!paymentService.UserHasRightToPayment(id, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            paymentService.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Removes a tag from a specific payment.
        /// </summary>
        /// <param name="tagId">The ID of the tag to remove.</param>
        /// <param name="paymentId">The ID of the payment from which to remove the tag.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The tag was removed successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified payment.</response>
        [HttpDelete, MapToApiVersion("1.0")]
        [Route("{paymentId}/tag/{tagId}"), MapToApiVersion("1.0")]
        public IActionResult RemoveTagFromPayment(int tagId, int paymentId)
        {
            if (paymentService.UserHasRightToPayment(paymentId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            tagService.RemoveTagFromPayment(tagId, paymentId);
            return Ok();
        }
    }
}