using MassTransit;

namespace BudgetManager.WebCore
{
    /// <summary>
    /// Config model for RabbitMq messaging service.
    /// </summary>
    public class RabbitMqConfig
    {
        /// <summary>
        /// RabitMq url
        /// </summary>
        public string RabbitMqUri { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// Configuration for message bus factory
        /// </summary>
        public Action<IRabbitMqBusFactoryConfigurator> EndpointsConfiguration { get; set; }
    }
}
