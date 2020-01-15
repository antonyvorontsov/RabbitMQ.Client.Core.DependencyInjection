using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// An interface of the service that contains logic of handling message receiving (consumption) events and passing those messages to the message handlers.
    /// </summary>
    public interface IMessageHandlingService
    {
        /// <summary>
        /// Handle message receiving event.
        /// </summary>
        /// <param name="eventArgs">Arguments of message receiving event.</param>
        /// <param name="queueService">An instance of the queue service <see cref="IQueueService"/>.</param>
        void HandleMessageReceivingEvent(BasicDeliverEventArgs eventArgs, IQueueService queueService);
    }
}