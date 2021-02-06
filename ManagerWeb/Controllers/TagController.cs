using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManagerWeb.Models.DTOs;
using System.Collections.Generic;
using ManagerWeb.Services;

namespace ManagerWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("tag")]
    public class TagController : ControllerBase
    {
        private readonly ITagService tagService;

        public TagController(ITagService tagService)
        {
            this.tagService = tagService;
        }

        [HttpGet]
        [Route("payment/all")]
        public ActionResult<IEnumerable<TagModel>> GetPaymentTags()
        {
            IEnumerable<TagModel> tags = this.tagService.GetPaymentTags();
            return Ok(tags);
        }

        [HttpPost]
        public IActionResult AddTagToPayment([FromBody] AddTagModel tagModel)
        {
            this.tagService.AddTagToPayment(tagModel);
            return Ok();
        }

        [HttpDelete]
        [Route("payment")]
        public IActionResult RemoveTagFromPayment([FromBody] int tagId, int paymentId)
        {
            this.tagService.RemoveTagFromPayment(tagId, paymentId);
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteTag(int tagId)
        {
            this.tagService.DeleteTag(tagId);
            return Ok();
        }
    }
}
