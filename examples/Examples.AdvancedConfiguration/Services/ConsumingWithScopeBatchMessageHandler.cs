using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Examples.AdvancedConfiguration.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace Examples.AdvancedConfiguration.Services
{
    public class ConsumingWithScopeBatchMessageHandler : BaseBatchMessageHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        
        public ConsumingWithScopeBatchMessageHandler(
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            IEnumerable<IBatchMessageHandlingFilter> batchMessageHandlingFilters,
            ILogger<ConsumingWithScopeBatchMessageHandler> logger,
            IServiceScopeFactory scopeFactory)
            : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, batchMessageHandlingFilters, logger)
        {
            _scopeFactory = scopeFactory;
        }

        public override ushort PrefetchCount { get; set; } = 15;

        public override string QueueName { get; set; } = "your.queue.name";

        public override async Task HandleMessages(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // Handler a batch of messages with db context.
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}