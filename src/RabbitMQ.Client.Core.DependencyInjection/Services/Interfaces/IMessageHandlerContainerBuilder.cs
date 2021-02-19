using System.Collections.Generic;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Interface of the service that build message handler containers collection.
    /// Those containers contain information about message handlers (all types) bound to the exchange.
    /// Container could be "general" if message handler has not been bound to the exchange (so it will "listen" regardless of the exchange).
    /// </summary>
    public interface IMessageHandlerContainerBuilder
    {
        /// <summary>
        /// Build message handler containers collection.
        /// </summary>
        /// <returns>Collection of message handler containers <see cref="MessageHandlerContainer"/>.</returns>
        IEnumerable<MessageHandlerContainer> BuildCollection();
    }
}