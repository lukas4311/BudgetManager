using System.Collections.Generic;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [ApiController]
    [Route("notification")]
    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        public NotificationController(IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService) : base(httpContextAccessor)
        {
            this.notificationService = notificationService;
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult<IEnumerable<NotificationModel>> GetNotifications()
        {
            IEnumerable<NotificationModel> tags = notificationService.GetUserNotifications(GetUserId());
            return Ok(tags);
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public IActionResult AddNotification([FromBody] NotificationModel tagModel)
        {
            if (!notificationService.UserHasRight(tagModel.UserIdentityId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            notificationService.Add(tagModel);
            return Ok();
        }

        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut]
        [Route("/{notificationId}/markAsDisplayed")]
        public IActionResult MarkAsDisplayed(int notificationId)
        {
            if (!notificationService.UserHasRight(notificationId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);

            notificationService.MarkAsDisplayed(notificationId);
            return Ok();
        }
    }
}