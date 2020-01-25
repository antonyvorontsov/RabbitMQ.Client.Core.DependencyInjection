using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Interface of asynchronous message handler.
    /// </summary>
    public interface IAsyncMessageHandler : IBaseMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        Task Handle(string message, string routingKey);
    }
}