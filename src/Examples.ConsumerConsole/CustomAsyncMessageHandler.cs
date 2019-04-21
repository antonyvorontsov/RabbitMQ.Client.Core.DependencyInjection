using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using System.Threading.Tasks;

namespace Examples.ConsumerConsole
{
    public class CustomAsyncMessageHandler : IAsyncMessageHandler
    {
        readonly ILogger<CustomAsyncMessageHandler> _logger;

        public CustomAsyncMessageHandler(ILogger<CustomAsyncMessageHandler> logger) =>
            _logger = logger;

        public async Task Handle(string message, string routingKey)
        {
            await Task.Run(() => _logger.LogInformation("A weird example of runnig something async."));
        }
    }
}