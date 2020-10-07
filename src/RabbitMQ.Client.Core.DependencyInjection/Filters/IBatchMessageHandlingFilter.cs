using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Filters
{
    /// <summary>
    /// An abstract filter that runs in the pipeline of message handling in batch handler.
    /// </summary>
    public interface IBatchMessageHandlingFilter
    {
        /// <summary>
        /// Execute filter logic.
        /// </summary>
        /// <param name="next">Next action.</param>
        /// <returns>Function that could process received message.</returns>
        Func<IEnumerable<BasicDeliverEventArgs>, CancellationToken, Task> Execute(Func<IEnumerable<BasicDeliverEventArgs>, CancellationToken, Task> next);
    }
}