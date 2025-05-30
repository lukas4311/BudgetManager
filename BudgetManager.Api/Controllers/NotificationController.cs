﻿using System.Collections.Generic;
using Asp.Versioning;
using BudgetManager.Domain.DTOs;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing user notifications in the Budget Manager application.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/notification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/json", "application/problem+json")]
    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
        /// <param name="notificationService">Service for notification-related operations.</param>
        public NotificationController(IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService) : base(httpContextAccessor)
        {
            this.notificationService = notificationService;
        }

        /// <summary>
        /// Retrieves all notifications for the current user.
        /// </summary>
        /// <returns>A collection of notification models for the current user.</returns>
        /// <response code="200">Returns the user's notifications successfully.</response>
        [HttpGet, MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<NotificationModel>> GetNotifications()
        {
            IEnumerable<NotificationModel> tags = notificationService.GetUserNotifications(GetUserId());
            return Ok(tags);
        }

        /// <summary>
        /// Adds a new notification to the system.
        /// </summary>
        /// <param name="tagModel">The notification model containing the notification details.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The notification was added successfully.</response>
        /// <response code="401">If the user doesn't have rights to add notifications for the specified user.</response>
        [HttpPost, MapToApiVersion("1.0")]
        public IActionResult AddNotification([FromBody] NotificationModel tagModel)
        {
            if (!notificationService.UserHasRight(tagModel.UserIdentityId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);
            notificationService.Add(tagModel);
            return Ok();
        }

        /// <summary>
        /// Marks a specific notification as displayed.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to mark as displayed.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        /// <response code="200">The notification was marked as displayed successfully.</response>
        /// <response code="401">If the user doesn't have rights to the specified notification.</response>
        [HttpPut, MapToApiVersion("1.0")]
        [Route("{notificationId}/markAsDisplayed"), MapToApiVersion("1.0")]
        public IActionResult MarkAsDisplayed(int notificationId)
        {
            if (!notificationService.UserHasRight(notificationId, GetUserId()))
                return StatusCode(StatusCodes.Status401Unauthorized);
            notificationService.MarkAsDisplayed(notificationId);
            return Ok();
        }
    }
}