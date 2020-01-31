using System;
using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// A model that contains information about message handler (by type) and its connection with exchange and routing keys as well as an order value.
    /// That model being registered in DI for each message handler.
    /// </summary>
    public class MessageHandlerOrderingModel
    {
        /// <summary>
        /// A type of the registered message handler.
        /// </summary>
        public Type MessageHandlerType { get; set; }

        /// <summary>
        /// An exchange which message handler bound to.
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// A collection of route patterns which message handler "listens".
        /// </summary>
        public IEnumerable<string> RoutePatterns { get; set; }

        /// <summary>
        /// The value of order.
        /// </summary>
        public int Order { get; set; }
    }
}