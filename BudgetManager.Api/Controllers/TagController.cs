using System.Collections.Generic;
using BudgetManager.Data.DataModels;
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

        [HttpGet]
        [Route("allUsed")]
        public ActionResult<IEnumerable<TagModel>> GetPaymentsTags()
        {
            IEnumerable<TagModel> tags = this.tagService.GetPaymentsTags(this.GetUserId());
            return Ok(tags);
        }

        [HttpPost]
        public IActionResult AddTagToPayment([FromBody] AddTagModel tagModel)
        {
            if (this.paymentService.UserHasRightToPayment(tagModel.PaymentId, this.GetUserId()))
                return this.StatusCode(StatusCodes.Status401Unauthorized);

            this.tagService.AddTagToPayment(tagModel);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteTag(int tagId)
        {
            this.tagService.Delete(tagId);
            return Ok();
        }
    }
}
