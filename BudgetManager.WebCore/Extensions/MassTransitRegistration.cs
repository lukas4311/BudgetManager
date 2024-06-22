using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetManager.WebCore.Extensions
{
    /// <summary>
    /// Masstransit extensions
    /// </summary>
    public static class MassTransitRegistration
    {
        /// <summary>
        /// Add Masstransit registration of RabbitMq
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rabbitMqConfig"></param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, RabbitMqConfig rabbitMqConfig)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(rabbitMqConfig.RabbitMqUri, c =>
                    {
                        if (!string.IsNullOrEmpty(rabbitMqConfig.User))
                        {
                            c.Username(rabbitMqConfig.User);
                            c.Password(rabbitMqConfig.Pass);
                        }
                    });

                    if (rabbitMqConfig.EndpointsConfiguration is not null)
                        rabbitMqConfig.EndpointsConfiguration?.Invoke(cfg);
                    else
                        cfg.ConfigureEndpoints(ctx);
                });
            });
            return services;
        }
    }
}
