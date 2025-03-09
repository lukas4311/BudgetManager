using System.Collections.Generic;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("tags")]
    public class TagController : BaseController
    {
        private readonly ITagService tagService;
        private readonly IPaymentService paymentService;

        public TagController(IHttpContextAccessor httpContextAccessor, ITagService tagService, IPaymentService paymentService) : base(httpContextAccessor)
        {
            this.tagService = tagService;
            this.paymentService = paymentService;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("allUsed")]
        public ActionResult<IEnumerable<TagModel>> GetPaymentsTags()
        {
            IEnumerable<TagModel> tags = tagService.GetPaymentsTags(GetUserId());
            return Ok(tags);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public IActionResult AddTagToPayment([FromBody] AddTagModel tagModel)
        {
            if (paymentService.UserHasRightToPayment(tagModel.PaymentId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            tagService.AddTagToPayment(tagModel);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete]
        public IActionResult DeleteTag(int tagId)
        {
            tagService.Delete(tagId);
            return Ok();
        }
    }
}
