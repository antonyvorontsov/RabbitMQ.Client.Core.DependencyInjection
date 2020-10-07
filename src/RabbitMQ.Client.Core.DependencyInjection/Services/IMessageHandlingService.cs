using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
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
        /// <param name="queueService">An instance of the queue service <see cref="IQueueService"/>.</param>
        Task HandleMessageReceivingEvent(BasicDeliverEventArgs eventArgs, IQueueService queueService);
        
        /// <summary>
        /// Handle message processing failure.
        /// </summary>
        /// <param name="exception">An occured exception.</param>
        /// <param name="eventArgs">Arguments of message receiving event.</param>
        /// <param name="queueService">An instance of the queue service <see cref="IQueueService"/>.</param>
        Task HandleMessageProcessingFailure(Exception exception, BasicDeliverEventArgs eventArgs, IQueueService queueService);
    }
}