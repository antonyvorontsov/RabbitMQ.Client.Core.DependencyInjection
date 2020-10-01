using System.Threading.Tasks;
using RabbitMQ.Client.Events;

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
        /// <param name="eventArgs">Message event args.</param>
        /// <param name="matchingRoute">Matching routing key.</param>
        Task Handle(BasicDeliverEventArgs eventArgs, string matchingRoute);
    }
}