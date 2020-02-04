using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection;

namespace Examples.AdvancedConfiguration.MessageHandlers
{
    public class CustomAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
    {
        public async Task Handle(string message, string routingKey, IQueueService queueService)
        {
            // The message handler does not do anything.
            // It is just an example.
            await Task.CompletedTask;
        }
    }
}