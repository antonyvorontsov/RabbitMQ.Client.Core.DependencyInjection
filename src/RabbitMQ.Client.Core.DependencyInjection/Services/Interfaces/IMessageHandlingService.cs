using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

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
        /// <param name="eventArgs">Arguments of message receiving event.</param>
        /// <param name="consumingService">An instance of consuming service <see cref="IConsumingService"/>.</param>
        Task HandleMessageReceivingEvent(BasicDeliverEventArgs eventArgs, IConsumingService consumingService);
        
        /// <summary>
        /// Handle message processing failure.
        /// </summary>
        /// <param name="exception">An occured exception.</param>
        /// <param name="eventArgs">Arguments of message receiving event.</param>
        /// <param name="consumingService">An instance of consuming service <see cref="IConsumingService"/>.</param>
        Task HandleMessageProcessingFailure(Exception exception, BasicDeliverEventArgs eventArgs, IConsumingService consumingService);
    }
}