namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Interface of message handler.
    /// </summary>
    public interface IMessageHandler : IBaseMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        void Handle(string message, string routingKey);
    }
}