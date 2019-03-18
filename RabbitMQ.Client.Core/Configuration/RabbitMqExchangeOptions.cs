using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Configuration
{
    /// <summary>
    /// Echange options.
    /// </summary>
    public class RabbitMqExchangeOptions
    {
        /// <summary>
        /// Exchange type.
        /// </summary>
        public string Type { get; set; } = "direct";

        /// <summary>
        /// Durable option.
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// AutoDelete option.
        /// </summary>
        public bool AutoDelete { get; set; } = false;
        
        /// <summary>
        /// Additional arguments.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Collection of queues connected to the exchange.
        /// </summary>
        public IList<RabbitMqQueueOptions> Queues { get; set; } = new List<RabbitMqQueueOptions>();    }
}