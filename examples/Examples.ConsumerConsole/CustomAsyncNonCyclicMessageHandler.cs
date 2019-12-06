using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using System.Threading.Tasks;

namespace Examples.ConsumerConsole
{
    public class CustomAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
    {
        readonly ILogger<CustomAsyncNonCyclicMessageHandler> _logger;

        public CustomAsyncNonCyclicMessageHandler(ILogger<CustomAsyncNonCyclicMessageHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(string message, string routingKey, IQueueService queueService)
        {
            _logger.LogInformation("A weird example of running something async.");
            var response = new { message, routingKey };
            await queueService.SendAsync(response, "exchange.name", "routing.key");
        }
    }
}