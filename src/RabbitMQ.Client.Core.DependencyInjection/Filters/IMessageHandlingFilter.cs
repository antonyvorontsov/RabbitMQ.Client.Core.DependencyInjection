using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Filters
{
    /// <summary>
    /// An abstract filter that runs in the pipeline of message handling.
    /// </summary>
    public interface IMessageHandlingFilter
    {
        /// <summary>
        /// Execute filter logic.
        /// </summary>
        /// <param name="next">Next action.</param>
        /// <returns>Function that could process received message.</returns>
        Func<BasicDeliverEventArgs, IConsumingService, Task> Execute(Func<BasicDeliverEventArgs, IConsumingService, Task> next);
    }
}