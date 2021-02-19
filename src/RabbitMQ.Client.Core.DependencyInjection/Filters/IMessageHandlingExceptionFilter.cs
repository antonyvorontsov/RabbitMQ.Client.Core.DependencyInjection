using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Filters
{
    /// <summary>
    /// An exception filter that runs in the pipeline that processes message handling failure.
    /// </summary>
    public interface IMessageHandlingExceptionFilter
    {
        /// <summary>
        /// Execute filter logic.
        /// </summary>
        /// <param name="next">Next action.</param>
        /// <returns>Function that could process occured exception.</returns>
        Func<Exception, BasicDeliverEventArgs, IConsumingService, Task> Execute(Func<Exception, BasicDeliverEventArgs, IConsumingService, Task> next);
    }
}