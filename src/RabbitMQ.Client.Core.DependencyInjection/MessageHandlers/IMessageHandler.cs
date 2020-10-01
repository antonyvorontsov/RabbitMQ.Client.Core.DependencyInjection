using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.MessageHandlers
{
    /// <summary>
    /// Interface of message handler.
    /// </summary>
    public interface IMessageHandler : IBaseMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <param name="matchingRoute">Matching routing key.</param>
        void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute);
    }
}