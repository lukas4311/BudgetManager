using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
