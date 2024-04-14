using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;

namespace BudgetManager.Services
{
    public class NotificationService : BaseService<NotificationModel, Notification, INotificationRepository>, INotificationService
    {
        public NotificationService(INotificationRepository repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
