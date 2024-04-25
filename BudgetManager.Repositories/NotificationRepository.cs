using BudgetManager.Data;
using BudgetManager.Data.DataModels;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="INotificationRepository" />
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(DataContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}