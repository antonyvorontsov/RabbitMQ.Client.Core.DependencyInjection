using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Service that contains logic of handling errors that occured in the pipeline of processing consumed messages.
    /// </summary>
    public interface IErrorProcessingService
    {
        /// <summary>
        /// Handle message processing failure.
        /// </summary>
        /// <param name="context">Model that contains consumed message alongside with additional actions <see cref="MessageHandlingContext"/>.</param>
        /// <param name="exception">An occured exception.</param>
        Task HandleMessageProcessingFailure(MessageHandlingContext context, Exception exception);
    }
}