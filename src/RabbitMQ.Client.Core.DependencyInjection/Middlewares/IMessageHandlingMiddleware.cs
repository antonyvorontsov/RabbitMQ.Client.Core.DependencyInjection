using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Middlewares
{
    /// <summary>
    /// Middleware that is used in message handling pipeline for the message handling service.
    /// </summary>
    public interface IMessageHandlingMiddleware
    {
        /// <summary>
        /// Middleware action for message processing.
        /// </summary>
        /// <param name="context">Message handling context.</param>
        /// <param name="next">Next action (middleware) in the constructed pipeline.</param>
        Task Handle(MessageHandlingContext context, Func<Task> next);
        
        /// <summary>
        /// Middleware action for exception handling.
        /// </summary>
        /// <param name="context">Message handling context.</param>
        /// <param name="exception">An exception occured while trying to process a consumed message.</param>
        /// <param name="next">Next action (middleware) in the constructed pipeline.</param>
        Task HandleError(MessageHandlingContext context, Exception exception, Func<Task> next);
    }
}