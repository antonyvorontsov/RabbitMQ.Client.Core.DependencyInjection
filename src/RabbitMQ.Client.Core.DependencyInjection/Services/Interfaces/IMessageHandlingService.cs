using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Service that contains logic of handling message receiving (consumption) events and passing those messages to the message handlers.
    /// </summary>
    public interface IMessageHandlingService
    {
        /// <summary>
        /// Handle message receiving event.
        /// </summary>
        /// <param name="context">Model that contains consumed message alongside with additional actions <see cref="MessageHandlingContext"/>.</param>
        Task HandleMessageReceivingEvent(MessageHandlingContext context);
        
        /// <summary>
        /// Handle message processing failure.
        /// </summary>
        /// <param name="context">Model that contains consumed message alongside with additional actions <see cref="MessageHandlingContext"/>.</param>
        /// <param name="exception">An occured exception.</param>
        Task HandleMessageProcessingFailure(MessageHandlingContext context, Exception exception);
    }
}