using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Events;

namespace Examples.ConsumerHost
{
    public class CustomMessageHandler : IMessageHandler
    {
        readonly ILogger<CustomMessageHandler> _logger;
        public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
        {
            _logger = logger;
        }

        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            _logger.LogInformation($"Handling message {eventArgs.GetMessage()} by routing key {matchingRoute}");
        }
    }
}