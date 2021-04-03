using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Examples.AdvancedConfiguration.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace Examples.AdvancedConfiguration.Services
{
    public class ConsumingWithProviderBatchMessageHandler : BaseBatchMessageHandler
    {
        private readonly ApplicationDbContext _dbContext;
        
        public ConsumingWithProviderBatchMessageHandler(
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            IEnumerable<IBatchMessageHandlingFilter> batchMessageHandlingFilters,
            ILoggingService loggingService,
            IServiceProvider provider)
            : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, batchMessageHandlingFilters, loggingService)
        {
            _dbContext = provider.GetRequiredService<ApplicationDbContext>();
        }

        public override ushort PrefetchCount { get; set; } = 15;

        public override string QueueName { get; set; } = "your.queue.name";

        public override async Task HandleMessages(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken)
        {
            // Handle messages.
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}