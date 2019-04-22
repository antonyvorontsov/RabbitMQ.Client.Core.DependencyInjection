using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;

namespace Examples.ConsumerHost
{
    public class CustomNonCyclicMessageHandler : INonCyclicMessageHandler
    {
        readonly ILogger<CustomNonCyclicMessageHandler> _logger;
        public CustomNonCyclicMessageHandler(ILogger<CustomNonCyclicMessageHandler> logger) =>
            _logger = logger;

        public void Handle(string message, string routingKey, IQueueService queueService)
        {
            _logger.LogInformation("Handling messages");
            var response = new { message, routingKey };
            queueService.Send(response, "exchange.name", "routing.key");
        }
    }
}