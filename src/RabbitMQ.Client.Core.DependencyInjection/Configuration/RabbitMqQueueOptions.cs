﻿using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// Queue options.
    /// </summary>
    public class RabbitMqQueueOptions
    {
        /// <summary>
        /// Queue name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Durable option.
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// Exclusive option.
        /// </summary>
        public bool Exclusive { get; set; }

        /// <summary>
        /// AutoDelete option.
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// Routing keys collection that queue "listens".
        /// </summary>
        public HashSet<string> RoutingKeys { get; set; } = new HashSet<string>();

        /// <summary>
        /// Additional arguments.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Declare queue in passive mode
        /// </summary>
        public bool PassiveMode { get; set; } = false;
    }
}