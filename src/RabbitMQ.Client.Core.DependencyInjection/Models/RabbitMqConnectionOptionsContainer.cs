using System;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// A model that contains a link between an instance of message queuing service and its configuration.
    /// </summary>
    public class RabbitMqConnectionOptionsContainer
    {
        /// <summary>
        /// Service type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Queuing service options container.
        /// </summary>
        public RabbitMqConnectionOptions Options { get; set; }
    }
}