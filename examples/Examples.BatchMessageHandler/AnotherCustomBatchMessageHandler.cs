using System;
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

        // You have to be aware that BaseBatchMessageHandler does not declare the specified queue. So if it does not exists an exception will be thrown.
        public override string QueueName { get; set; } = "another.queue.name";

        // This thing will fire message handling if there are not enough messages, but timeout is already off.
        public override TimeSpan? MessageHandlingPeriod { get; set; } = TimeSpan.FromMilliseconds(500);

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