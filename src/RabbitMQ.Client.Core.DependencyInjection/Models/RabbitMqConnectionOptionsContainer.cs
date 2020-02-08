using System;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// A model that contains a link between an instance of message queuing service and its configuration.
    /// A queuing service can be of type <see cref="IQueueService"/>, <see cref="IConsumingService"/> or <see cref="IProducingService"/>.
    /// </summary>
    public class RabbitMqConnectionOptionsContainer
    {
        /// <summary>
        /// Queuing service identifier.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Queuing service options container.
        /// </summary>
        public RabbitMqConnectionOptions Options { get; set; }
    }
}