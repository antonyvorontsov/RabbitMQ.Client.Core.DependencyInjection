using RabbitMQ.Client.Core.DependencyInjection;

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