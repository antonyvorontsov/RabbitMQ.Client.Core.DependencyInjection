using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Examples.BatchMessageHandler
{
    public class AnotherCustomBatchMessageHandler : RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers.BatchMessageHandler
    {
        readonly ILogger<AnotherCustomBatchMessageHandler> _logger;

        public AnotherCustomBatchMessageHandler(
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<AnotherCustomBatchMessageHandler> logger)
            : base(batchConsumerConnectionOptions, logger)
        {
            _logger = logger;
        }

        protected override ushort PrefetchCount { get; set; } = 5;

        protected override string QueueName { get; set; } = "another.queue.name";

        protected override Task HandleMessage(IEnumerable<string> messages, CancellationToken cancellationToken)
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