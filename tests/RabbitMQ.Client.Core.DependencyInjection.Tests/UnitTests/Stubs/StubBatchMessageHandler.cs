using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests.Stubs
{
    public class StubBatchMessageHandler : BaseBatchMessageHandler
    {
        readonly ILogger<StubBatchMessageHandler> _logger;
        
        public StubBatchMessageHandler(
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<StubBatchMessageHandler> logger)
            : base(batchConsumerConnectionOptions, logger)
        {
            _logger = logger;
        }
        
        protected override ushort PrefetchCount { get; set; } = 3;
        
        protected override string QueueName { get; set; } = "queue.name";
        
        protected override Task HandleMessages(IEnumerable<ReadOnlyMemory<byte>> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                var stringifiedMessage = Encoding.UTF8.GetString(message.ToArray());
                _logger.LogInformation(stringifiedMessage);
            }
            return Task.CompletedTask;
        }
    }
}