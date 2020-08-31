using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManagerWeb.Models.DTOs;
using System.Collections.Generic;
using ManagerWeb.Services;

namespace ManagerWeb.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        private readonly ITagService tagService;

        public TagController(ITagService tagService)
        {
            this.tagService = tagService;
        }

        [HttpGet]
        public JsonResult GetPaymentTags()
        {
            IEnumerable<TagModel> tags = this.tagService.GetPaymentTags();
            return Json(new { tags });
        }

        [HttpPost]
        public JsonResult AddTagToPayment([FromBody]AddTagModel tagModel)
        {
            this.tagService.AddTagToPayment(tagModel);
            return Json(new {});
        }

        [HttpDelete]
        public JsonResult RemoveTagFromPayment()
        {
            return Json(new {});
        }

        [HttpDelete]
        public JsonResult DeleteTag(int tagId)
        {
            this.tagService.DeleteTag(tagId);
            return Json(new {});
        }
    }
}
