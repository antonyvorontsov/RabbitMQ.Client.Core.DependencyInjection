using System;
using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Message handler router model.
    /// </summary>
    public class MessageHandlerRouter
    {
        /// <summary>
        /// Message Handler Type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Collection of route patterns (routing keys) that handler will be "listening".
        /// </summary>
        public List<string> RoutePatterns { get; set; } = new List<string>();
    }
}