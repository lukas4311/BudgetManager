using System.Linq;
using Autofac;

namespace BudgetManager.Services.Extensions
{
    public static class DiRegisterExtension
    {
        public static void RegisterServices(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterAssemblyTypes(typeof(DiRegisterExtension).Assembly)
                .Where(t => t.Namespace == "BudgetManager.Services")
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        }
    }
}
