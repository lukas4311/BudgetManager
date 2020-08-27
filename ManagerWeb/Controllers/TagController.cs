using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Repository;
using System.Linq;
using ManagerWeb.Models.DTOs;
using System.Collections.Generic;

namespace ManagerWeb.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly ITagRepository tagRepository;

        public TagController(ILogger<HomeController> logger, ITagRepository tagRepository)
        {
            this.logger = logger;
            this.tagRepository = tagRepository;
        }

        public JsonResult GetPaymentTags()
        {
            IEnumerable<TagModel> tags = this.tagRepository.FindAll().Select(t => new TagModel
            {
                Code = t.Code,
                Id = t.Id
            });

            return Json(new { tags });
        }
    }
}
