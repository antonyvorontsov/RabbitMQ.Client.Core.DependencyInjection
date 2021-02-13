using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace Examples.BatchMessageHandler
{
    public class CustomBatchMessageHandler : BaseBatchMessageHandler
    {
        private readonly ILogger<CustomBatchMessageHandler> _logger;

        public CustomBatchMessageHandler(
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            IEnumerable<IBatchMessageHandlingFilter> batchMessageHandlingFilters,
            ILogger<CustomBatchMessageHandler> logger)
            : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, batchMessageHandlingFilters, logger)
        {
            _logger = logger;
        }

        public override ushort PrefetchCount { get; set; } = 3;

        // You have to be aware that BaseBatchMessageHandler does not declare the specified queue. So if it does not exists an exception will be thrown.
        public override string QueueName { get; set; } = "queue.name";

        public override Task HandleMessages(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling a batch of messages.");
            foreach (var message in messages)
            {
                _logger.LogInformation(message.GetMessage());
            }
            return Task.CompletedTask;
        }
    }
}