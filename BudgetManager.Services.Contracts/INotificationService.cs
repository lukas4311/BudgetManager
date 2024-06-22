using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    /// <summary>
    /// Service for managing notifications.
    /// </summary>
    public interface INotificationService : IBaseService<NotificationModel, Notification>
    {
        /// <summary>
        /// Gets the notifications for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An enumerable collection of notification models for the user.</returns>
        IEnumerable<NotificationModel> GetUserNotifications(int userId);

        /// <summary>
        /// Marks a notification as displayed.
        /// </summary>
        /// <param name="notificationId">The ID of the notification to mark as displayed.</param>
        void MarkAsDisplayed(int notificationId);

        /// <summary>
        /// Checks if a user has the right to access a specific notification.
        /// </summary>
        /// <param name="notificationId">The ID of the notification.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the user has the right to access the notification, otherwise false.</returns>
        bool UserHasRight(int notificationId, int userId);
    }
}
