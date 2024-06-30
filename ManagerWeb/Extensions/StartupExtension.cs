using BudgetManager.Data;
using Infl = BudgetManager.InfluxDbData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BudgetManager.Repository;
using BudgetManager.Data.DataModels;

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
            services.AddTransient<IRepository<PaymentCategory>, Repository<PaymentCategory>>();
            services.AddTransient<IRepository<PaymentType>, Repository<PaymentType>>();
            services.AddTransient<IRepository<Payment>, Repository<Payment>>();
            services.AddTransient<IRepository<BankAccount>, Repository<BankAccount>>();
            services.AddTransient<IRepository<UserIdentity>, Repository<UserIdentity>>();
            services.AddTransient<IRepository<PaymentTag>, Repository<PaymentTag>>();
            services.AddTransient<IRepository<Tag>, Repository<Tag>>();
            services.AddTransient<IRepository<Budget>, Repository<Budget>>();
            services.AddTransient<IRepository<CryptoTradeHistory>, Repository<CryptoTradeHistory>>();
            services.AddTransient<IRepository<CryptoTicker>, Repository<CryptoTicker>>();
            services.AddTransient<IRepository<CurrencySymbol>, Repository<CurrencySymbol>>();
            services.AddTransient< IRepository<InterestRate>, Repository<InterestRate>>();
        }

        internal static void ConfigureInfluxRepositories(this IServiceCollection services)
        {
            services.AddTransient<InfluxDbData.IRepository<Infl.ForexData>, BudgetManager.InfluxDbData.Repository<Infl.ForexData>>();
            services.AddTransient<InfluxDbData.IRepository<Infl.CryptoData>, BudgetManager.InfluxDbData.Repository<Infl.CryptoData>>();
        }
    }
}
