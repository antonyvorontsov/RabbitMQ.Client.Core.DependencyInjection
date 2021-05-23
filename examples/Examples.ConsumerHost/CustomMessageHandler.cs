using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Examples.ConsumerHost
{
    public class CustomMessageHandler : IMessageHandler
    {
        private readonly ILogger<CustomMessageHandler> _logger;
        public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
        {
            _logger = logger;
        }
        
        public void Handle(MessageHandlingContext context, string matchingRoute)
        {
            _logger.LogInformation($"Handling message {context.Message.GetMessage()} by routing key {matchingRoute}");
        }
    }
}