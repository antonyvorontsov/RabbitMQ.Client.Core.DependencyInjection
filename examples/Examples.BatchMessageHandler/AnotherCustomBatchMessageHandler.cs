using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Examples.BatchMessageHandler
{
    public class AnotherCustomBatchMessageHandler : BaseBatchMessageHandler
    {
        readonly ILogger<AnotherCustomBatchMessageHandler> _logger;
        
        public AnotherCustomBatchMessageHandler(
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<AnotherCustomBatchMessageHandler> logger)
            : base(batchConsumerConnectionOptions, logger)
        {
            _logger = logger;
        }

        protected override TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);
        
        protected override ushort PrefetchCount { get; set; } = 5;
        
        protected override string QueueName { get; set; } = "another.queue.name";
        
        protected override Task HandleMessages()
        {
            _logger.LogInformation($"Handling from {typeof(AnotherCustomBatchMessageHandler)} messages");
            return Task.CompletedTask;
        }
    }
}