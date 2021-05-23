using RabbitMQ.Client.Core.DependencyInjection.Models;

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
        /// <param name="context">Message handling context.</param>
        /// <param name="matchingRoute">Matching routing key.</param>
        void Handle(MessageHandlingContext context, string matchingRoute);
    }
}