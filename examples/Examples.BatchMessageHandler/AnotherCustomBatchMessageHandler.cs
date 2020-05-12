using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace Examples.BatchMessageHandler
{
    public class AnotherCustomBatchMessageHandler : RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers.BatchMessageHandler
    {
        readonly ILogger<AnotherCustomBatchMessageHandler> _logger;

        public AnotherCustomBatchMessageHandler(
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<AnotherCustomBatchMessageHandler> logger)
            : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, logger)
        {
            _logger = logger;
        }

        public override ushort PrefetchCount { get; set; } = 5;

        public override string QueueName { get; set; } = "another.queue.name";

        public override Task HandleMessages(IEnumerable<string> messages, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling a batch of messages.");
            foreach (var message in messages)
            {
                _logger.LogInformation(message);
            }
            return Task.CompletedTask;
        }
    }
}