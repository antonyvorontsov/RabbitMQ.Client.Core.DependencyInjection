using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomAsyncMessageHandler : IAsyncMessageHandler
    {
        public async Task Handle(MessageHandlingContext context, string matchingRoute)
        {
            // The message handler does not do anything.
            // It is just an example.
            await Task.CompletedTask;
        }
    }
}