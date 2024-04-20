using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class NotificationService : BaseService<NotificationModel, Notification, INotificationRepository>, INotificationService
    {
        private readonly INotificationRepository repository;

        public NotificationService(INotificationRepository repository, IMapper mapper) : base(repository, mapper)
        {
            this.repository = repository;
        }
        
        public IEnumerable<NotificationModel> GetUserNotifications(int userId)
        {
            return repository.FindByCondition(u => u.Id == userId)
                .Select(t => mapper.Map<NotificationModel>(t));
        }
        
        public bool UserHasRight(int notificationId, int userId) => this.repository.FindByCondition(a => a.Id == notificationId && a.UserIdentityId == userId).Count() == 1;
    }
}
