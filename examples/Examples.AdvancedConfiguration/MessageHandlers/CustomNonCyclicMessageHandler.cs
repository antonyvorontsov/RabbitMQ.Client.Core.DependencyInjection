using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomNonCyclicMessageHandler : INonCyclicMessageHandler
    {
        public void Handle(string message, string routingKey, IQueueService queueService)
        {
            // The message handler does not do anything.
            // It is just an example.
        }
    }
}