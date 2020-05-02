using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Examples.BatchMessageHandler
{
    public class CustomBatchMessageHandler : BaseBatchMessageHandler
    {
        readonly ILogger<CustomBatchMessageHandler> _logger;
        
        public CustomBatchMessageHandler(
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<CustomBatchMessageHandler> logger)
            : base(batchConsumerConnectionOptions, logger)
        {
            _logger = logger;
        }

        protected override TimeSpan Period { get; set; } = TimeSpan.FromMinutes(10);
        
        protected override ushort PrefetchCount { get; set; } = 5;

        protected override string QueueName { get; set; } = "queue.name";
        
        protected override Task HandleMessages()
        {
            _logger.LogInformation("Handling messages");
            return Task.CompletedTask;
        }
    }
}