using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Events;

namespace Examples.ConsumerConsole
{
    public class CustomAsyncMessageHandler : IAsyncMessageHandler
    {
        private readonly ILogger<CustomAsyncMessageHandler> _logger;

        public CustomAsyncMessageHandler(ILogger<CustomAsyncMessageHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            _logger.LogInformation($"A weird example of running something async with message {eventArgs.GetMessage()} that has been received by {matchingRoute}.");
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}