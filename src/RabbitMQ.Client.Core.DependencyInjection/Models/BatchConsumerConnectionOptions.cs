using System;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// A configuration model that contains client connection options for each batch message handler.
    /// </summary>
    public class BatchConsumerConnectionOptions
    {
        /// <summary>
        /// Type of batch message handler.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Consumer client connection options.
        /// </summary>
        public RabbitMqServiceOptions ServiceOptions { get; set; }
    }
}