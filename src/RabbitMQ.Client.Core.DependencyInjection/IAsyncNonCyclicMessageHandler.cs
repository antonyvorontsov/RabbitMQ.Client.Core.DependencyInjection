using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Interface of a non-cycling async message handler.
    /// </summary>
    public interface IAsyncNonCyclicMessageHandler : IBaseMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="queueService">Instance of service <see cref="IQueueService"/> that can send messages.</param>
        Task Handle(string message, string routingKey, IQueueService queueService);
    }
}