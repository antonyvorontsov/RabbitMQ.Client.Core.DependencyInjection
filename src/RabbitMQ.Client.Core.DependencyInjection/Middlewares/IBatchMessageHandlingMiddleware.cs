using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Middlewares
{
    /// <summary>
    /// Middleware that is used in message handling pipeline for batch message handlers.
    /// </summary>
    public interface IBatchMessageHandlingMiddleware
    {
        /// <summary>
        /// Handle a batch of messages.
        /// </summary>
        /// <param name="messages">A collection of messages.</param>
        /// <param name="next">Next action (middleware) in the constructed pipeline.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task Handle(IEnumerable<BasicDeliverEventArgs> messages, Func<Task> next, CancellationToken cancellationToken);
    }
}