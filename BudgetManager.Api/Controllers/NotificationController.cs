using System.Collections.Generic;
using BudgetManager.Data.DataModels;
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

        public NotificationController(IHttpContextAccessor httpContextAccessor, INotificationService notificationService) : base(httpContextAccessor)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<NotificationModel>> GetNotifications()
        {
            IEnumerable<NotificationModel> tags = notificationService.GetUserNotifications(GetUserId());
            return Ok(tags);
        }
    }
}
