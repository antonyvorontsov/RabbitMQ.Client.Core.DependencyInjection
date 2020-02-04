using RabbitMQ.Client.Core.DependencyInjection;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomMessageHandler : IMessageHandler
    {
        public void Handle(string message, string routingKey)
        {
            // The message handler does not do anything.
            // It is just an example.
        }
    }
}