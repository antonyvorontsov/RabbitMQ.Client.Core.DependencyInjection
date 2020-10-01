using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Events;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomAsyncMessageHandler : IAsyncMessageHandler
    {
        public async Task Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            // The message handler does not do anything.
            // It is just an example.
            await Task.CompletedTask;
        }
    }
}