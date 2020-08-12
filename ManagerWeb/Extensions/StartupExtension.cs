using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;

namespace ManagerWeb.Extensions
{
    internal static class StartupExtension
    {
        internal static void ConfigureDataContext(this IServiceCollection services, string connectionString)
        {
            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(connectionString);
            services.AddTransient<DataContext>(_ => new DataContext(optionsBuilder.Options));
        }

        internal static void ConfigureIoCRepositories(this IServiceCollection services)
        {
            services.AddTransient<IPaymentCategoryRepository, PaymentCategoryRepository>();
            services.AddTransient<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IBankAccountRepository, BankAccountRepository>();
            services.AddTransient<IUserIdentityRepository, UserIdentityRepository>();
        }
    }
}
