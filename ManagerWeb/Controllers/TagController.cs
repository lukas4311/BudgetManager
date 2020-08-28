using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Repository;
using System.Linq;
using ManagerWeb.Models.DTOs;
using System.Collections.Generic;
using System;
using Data.DataModels;
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
        public JsonResult AddTag(TagModel tagModel)
        {
            this.tagService.AddTag(tagModel);

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
