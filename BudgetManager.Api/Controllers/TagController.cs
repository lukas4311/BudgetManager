using System.Collections.Generic;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("tag")]
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
        [Route("payment/all")]
        public ActionResult<IEnumerable<TagModel>> GetPaymentTags()
        {
            IEnumerable<TagModel> tags = this.tagService.GetPaymentTags(this.GetUserId());
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
        [Route("payment")]
        public IActionResult RemoveTagFromPayment([FromBody] int tagId, int paymentId)
        {
            if (this.paymentService.UserHasRightToPayment(paymentId, this.GetUserId()))
                return this.StatusCode(StatusCodes.Status401Unauthorized);

            this.tagService.RemoveTagFromPayment(tagId, paymentId);
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
