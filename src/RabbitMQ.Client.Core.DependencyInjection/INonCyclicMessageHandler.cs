namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Interface of a non-cycling message handler.
    /// </summary>
    public interface INonCyclicMessageHandler : IBaseMessageHandler
    {
        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="queueService">Instance of service <see cref="IQueueService"/> that can send messages.</param>
        void Handle(string message, string routingKey, IQueueService queueService);
    }
}