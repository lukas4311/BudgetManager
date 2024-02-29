using MassTransit;

namespace BudgetManager.WebCore
{
    public class RabbitMqConfig
    {
        public string RabbitMqUri { get; set; }

        public string User { get; set; }

        public string Pass { get; set; }

        public Action<IRabbitMqBusFactoryConfigurator> EndpointsConfiguration { get; set; }
    }
}
