using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Repository;
using System.Linq;
using ManagerWeb.Models.DTOs;
using System.Collections.Generic;
using System;
using Data.DataModels;

namespace ManagerWeb.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        private const string AlreadyExist = "Tag with this code already exists";
        private const string DoesntExists = "Tag doesn't exists";
        private readonly ILogger<HomeController> logger;
        private readonly ITagRepository tagRepository;

        public TagController(ILogger<HomeController> logger, ITagRepository tagRepository)
        {
            this.logger = logger;
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public JsonResult GetPaymentTags()
        {
            IEnumerable<TagModel> tags = this.tagRepository.FindAll().Select(t => new TagModel
            {
                Code = t.Code,
                Id = t.Id
            });

            return Json(new { tags });
        }

        [HttpPost]
        public JsonResult AddTag(TagModel tagModel)
        {
            bool tagCodeExists = this.tagRepository.FindByCondition(t => string.Compare(t.Code, tagModel.Code, true) == 0).Any();

            if (tagCodeExists)
                throw new ArgumentException(AlreadyExist);

            return Json(new {});
        }

        [HttpDelete]
        public JsonResult DeleteTag(int tagId)
        {
            Tag tag = this.tagRepository.FindByCondition(t => t.Id == tagId).SingleOrDefault();

            if (tag == null)
                throw new ArgumentException(DoesntExists);

            return Json(new {});
        }
    }
}
