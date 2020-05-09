using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers
{
    /// <summary>
    /// Batch message handler that handles messages and passes them into HandleMessage method as strings.
    /// </summary>
    public abstract class BatchMessageHandler : BaseBatchMessageHandler
    {
        protected BatchMessageHandler(
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<BatchMessageHandler> logger)
            : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, logger)
        {
        }

        /// <summary>
        /// Handle a batch of messages.
        /// </summary>
        /// <param name="messages">A collection of messages as bytes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public override async Task HandleMessages(IEnumerable<ReadOnlyMemory<byte>> messages, CancellationToken cancellationToken)
        {
            var decodedMessages = messages.Select(x => Encoding.UTF8.GetString(x.ToArray()));
            await HandleMessages(decodedMessages, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle a batch of decoded messages.
        /// </summary>
        /// <param name="messages">A collection of messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public abstract Task HandleMessages(IEnumerable<string> messages, CancellationToken cancellationToken);
    }
}