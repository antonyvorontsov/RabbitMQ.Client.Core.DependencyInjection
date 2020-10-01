using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Events;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomMessageHandler : IMessageHandler
    {
        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            // The message handler does not do anything.
            // It is just an example.
        }
    }
}