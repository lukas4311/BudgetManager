using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetManager.WebCore.Extensions
{
    public static class MassTransitRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, RabbitMqConfig rabbitMqConfig)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rabbitMqConfig.RabbitMqUri, "/", c =>
                    {
                        c.Username(rabbitMqConfig.User);
                        c.Password(rabbitMqConfig.Pass);
                    });
                    cfg.ConfigureEndpoints(ctx);
                });
            });
            return services;
        }
    }
}
