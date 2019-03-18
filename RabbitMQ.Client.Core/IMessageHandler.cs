using System.Collections.Generic;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Interface of a service that handle messages.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Collection of routing keys which that handler will be "listening".
        /// </summary>
        IEnumerable<string> RoutingKeys { get; set; }

        /// <summary>
        /// Handle message from a queue.
        /// </summary>
        /// <param name="message">Json message.</param>
        /// <param name="routingKey">Routing key.</param>
        void Handle(string message, string routingKey);
    }
}