using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace Examples.ConsumerHost
{
    public class CustomMessageHandler : IMessageHandler
    {
        readonly ILogger<CustomMessageHandler> _logger;
        public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
        {
            _logger = logger;
        }

        public void Handle(string message, string routingKey)
        {
            _logger.LogInformation($"Handling message {message} by routing key {routingKey}");
        }
    }
}