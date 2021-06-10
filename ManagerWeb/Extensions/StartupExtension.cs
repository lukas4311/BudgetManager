using BudgetManager.Data;
using BudgetManager.InfluxDbData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BudgetManager.Repository;

namespace BudgetManager.ManagerWeb.Extensions
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
            services.AddTransient<IPaymentTagRepository, PaymentTagRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IBudgetRepository, BudgetRepository>();
            services.AddTransient<ICryptoTradeHistoryRepository, CryptoTradeHistoryRepository>();
            services.AddTransient<ICryptoTickerRepository, CryptoTickerRepository>();
            services.AddTransient<ICurrencySymbolRepository, CurrencySymbolRepository>();
            services.AddTransient<IInterestRateRepository, InterestRateRepository>();
        }

        internal static void ConfigureInfluxRepositories(this IServiceCollection services)
        {
            services.AddTransient<InfluxDbData.IRepository<ForexData>, InfluxDbData.Repository<ForexData>>();
            services.AddTransient<InfluxDbData.IRepository<CryptoData>, InfluxDbData.Repository<CryptoData>>();
        }
    }
}
