using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;

namespace Examples.ConsumerHost
{
    public class CustomNonCyclicMessageHandler : INonCyclicMessageHandler
    {
        readonly ILogger<CustomNonCyclicMessageHandler> _logger;
        public CustomNonCyclicMessageHandler(ILogger<CustomNonCyclicMessageHandler> logger)
        {
            _logger = logger;
        }

        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute, IQueueService queueService)
        {
            _logger.LogInformation("Handling messages");
            var response = new { message = eventArgs.GetMessage(), matchingRoute };
            queueService.Send(response, "exchange.name", "routing.key");
        }
    }
}