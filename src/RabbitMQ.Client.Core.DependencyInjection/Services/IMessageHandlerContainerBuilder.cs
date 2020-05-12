using System.Collections.Generic;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <summary>
    /// Interface of the service that build message handler containers collection.
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