using BudgetManager.ManagerWeb.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetManager.ManagerWeb.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
        }
    }
}
