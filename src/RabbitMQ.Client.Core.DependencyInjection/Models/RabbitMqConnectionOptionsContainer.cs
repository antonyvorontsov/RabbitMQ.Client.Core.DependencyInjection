using System;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// A model that contains a link between an instance of message queueing service and its configuration.
    /// A queueing service can be of type <see cref="IQueueService"/>, <see cref="IConsumingService"/> or <see cref="IProducingService"/>.
    /// </summary>
    public class RabbitMqConnectionOptionsContainer
    {
        /// <summary>
        /// Queueing service identifier.
        /// </summary>
        public Guid Guid { get; set; }
        
        /// <summary>
        /// Queueing service options container.
        /// </summary>
        public RabbitMqConnectionOptions Options { get; set; }
    }
}