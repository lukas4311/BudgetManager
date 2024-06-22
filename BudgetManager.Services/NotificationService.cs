using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    /// <inheritdoc/>
    public class NotificationService : BaseService<NotificationModel, Notification, INotificationRepository>, INotificationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="repository">The notification repository.</param>
        /// <param name="mapper">The mapper for converting between models and entities.</param>
        public NotificationService(INotificationRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }

        /// <inheritdoc/>
        public IEnumerable<NotificationModel> GetUserNotifications(int userId)
        {
            return repository.FindByCondition(u => u.UserIdentityId == userId)
                .Select(t => mapper.Map<NotificationModel>(t));
        }

        /// <inheritdoc/>
        public void MarkAsDisplayed(int notificationId)
        {
            Notification notification = repository.Get(notificationId);
            notification.IsDisplayed = true;
            repository.Save();
        }

        /// <inheritdoc/>
        public bool UserHasRight(int notificationId, int userId)
            => this.repository.FindByCondition(a => a.Id == notificationId && a.UserIdentityId == userId).Count() == 1;
    }
}
