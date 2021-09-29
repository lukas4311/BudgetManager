using Autofac;

namespace BudgetManager.Repository.Extensions
{
    public static class DiRegisterExtension
    {
        public static void RegisterRepositories(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterAssemblyTypes(typeof(DiRegisterExtension).Assembly)
                .Where(t => t.Namespace == "BudgetManager.Repository")
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        }
    }
}
