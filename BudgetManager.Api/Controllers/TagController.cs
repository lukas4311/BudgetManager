using System.Collections.Generic;
using Asp.Versioning;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller for managing tags related to payments.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class TagController : BaseController
    {
        private readonly ITagService tagService;
        private readonly IPaymentService paymentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="tagService">Service for managing tags.</param>
        /// <param name="paymentService">Service for managing payment-related operations.</param>
        public TagController(IHttpContextAccessor httpContextAccessor, ITagService tagService, IPaymentService paymentService)
            : base(httpContextAccessor)
        {
            this.tagService = tagService;
            this.paymentService = paymentService;
        }

        /// <summary>
        /// Retrieves all tags that have been used in payments by the current user.
        /// </summary>
        /// <returns>A list of tags used in payments.</returns>
        [HttpGet, MapToApiVersion("1.0")]
        [Route("allUsed"), MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<TagModel>> GetPaymentsTags()
        {
            IEnumerable<TagModel> tags = tagService.GetPaymentsTags(GetUserId());
            return Ok(tags);
        }

        /// <summary>
        /// Adds a tag to a specific payment if the user has the required permissions.
        /// </summary>
        /// <param name="tagModel">The tag model containing tag details and the associated payment ID.</param>
        /// <returns>A status code indicating success or failure.</returns>
        [HttpPost, MapToApiVersion("1.0")]
        public IActionResult AddTagToPayment([FromBody] AddTagModel tagModel)
        {
            if (paymentService.UserHasRightToPayment(tagModel.PaymentId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            tagService.AddTagToPayment(tagModel);
            return Ok();
        }

        /// <summary>
        /// Deletes a tag by its identifier.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to delete.</param>
        /// <returns>A status code indicating success.</returns>
        [HttpDelete, MapToApiVersion("1.0")]
        public IActionResult DeleteTag(int tagId)
        {
            tagService.Delete(tagId);
            return Ok();
        }
    }
}
