using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.MessageHandlers
{
    /// <summary>
    /// Interface of a non-cycling message handler.
    /// </summary>
    public interface INonCyclicMessageHandler : IBaseMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <param name="matchingRoute">Matching routing key.</param>
        /// <param name="queueService">Instance of service <see cref="IQueueService"/> that can send messages.</param>
        void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute, IQueueService queueService);
    }
}