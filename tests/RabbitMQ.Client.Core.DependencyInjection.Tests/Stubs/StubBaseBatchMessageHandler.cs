using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubBaseBatchMessageHandler : BaseBatchMessageHandler
    {
        private readonly IStubCaller _caller;

        public StubBaseBatchMessageHandler(
            IStubCaller caller,
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            IEnumerable<IBatchMessageHandlingFilter> batchMessageHandlingFilters,
            ILogger<StubBaseBatchMessageHandler> logger)
            : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, batchMessageHandlingFilters, logger)
        {
            _caller = caller;
        }

        public override ushort PrefetchCount { get; set; }

        public override string QueueName { get; set; }
        
        public override TimeSpan? MessageHandlingPeriod { get; set; }

        public override Task HandleMessages(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken)
        {
            foreach (var message in messages)
            {
                _caller.Call(message.Body);
            }
            _caller.EmptyCall();
            return Task.CompletedTask;
        }
    }
}