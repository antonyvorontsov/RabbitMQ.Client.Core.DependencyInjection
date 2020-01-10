using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// An interface of the service that contains logic of handling message consumption events and passing them to the message handlers.
    /// </summary>
    public interface IMessageHandlingService
    {
        /// <summary>
        /// Handle message consumption event.
        /// </summary>
        /// <param name="eventArgs">Arguments of message consumption event.</param>
        /// <param name="queueService">An instance of the queue service <see cref="IQueueService"/>.</param>
        void HandleMessage(BasicDeliverEventArgs @eventArgs, IQueueService queueService);
    }
}