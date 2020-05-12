using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubBaseBatchMessageHandler : BaseBatchMessageHandler
    {
        readonly IStubCaller _caller;

        public StubBaseBatchMessageHandler(
            IStubCaller caller,
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<StubBaseBatchMessageHandler> logger)
            : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, logger)
        {
            _caller = caller;
        }

        public override ushort PrefetchCount { get; set; }

        public override string QueueName { get; set; }

        public override Task HandleMessages(IEnumerable<ReadOnlyMemory<byte>> messages, CancellationToken cancellationToken)
        {
            _caller.EmptyCall();
            foreach (var message in messages)
            {
                _caller.Call(message);
            }
            return Task.CompletedTask;
        }
    }
}