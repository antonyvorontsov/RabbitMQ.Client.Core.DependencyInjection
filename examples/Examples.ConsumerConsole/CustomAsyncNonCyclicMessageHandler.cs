using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Events;

namespace Examples.ConsumerConsole
{
    public class CustomAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
    {
        private readonly ILogger<CustomAsyncNonCyclicMessageHandler> _logger;

        public CustomAsyncNonCyclicMessageHandler(ILogger<CustomAsyncNonCyclicMessageHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(BasicDeliverEventArgs eventArgs, string matchingRoute, IQueueService queueService)
        {
            _logger.LogInformation($"A weird example of running something async. Message has been received by routing key {matchingRoute}");
            var response = eventArgs.GetPayload<Message>();
            const string routingKey = "another.routing.key";
            _logger.LogInformation($"Sending the same message to {routingKey}.");
            await queueService.SendAsync(response, "exchange.name", routingKey);
        }
    }
}