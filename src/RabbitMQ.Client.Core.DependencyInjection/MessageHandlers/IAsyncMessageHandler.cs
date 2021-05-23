using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.MessageHandlers
{
    /// <summary>
    /// Interface of asynchronous message handler.
    /// </summary>
    public interface IAsyncMessageHandler : IBaseMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="context">Message handling context.</param>
        /// <param name="matchingRoute">Matching routing key.</param>
        Task Handle(MessageHandlingContext context, string matchingRoute);
    }
}