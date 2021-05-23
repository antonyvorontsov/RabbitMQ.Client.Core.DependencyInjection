using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomMessageHandler : IMessageHandler
    {
        public void Handle(MessageHandlingContext context, string matchingRoute)
        {
            // The message handler does not do anything.
            // It is just an example.
        }
    }
}