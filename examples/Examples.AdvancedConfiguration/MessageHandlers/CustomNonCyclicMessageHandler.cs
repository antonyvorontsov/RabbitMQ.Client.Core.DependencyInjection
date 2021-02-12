using RabbitMQ.Client.Events;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomNonCyclicMessageHandler : INonCyclicMessageHandler
    {
        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute, IQueueService queueService)
        {
            // The message handler does not do anything.
            // It is just an example.
        }
    }
}