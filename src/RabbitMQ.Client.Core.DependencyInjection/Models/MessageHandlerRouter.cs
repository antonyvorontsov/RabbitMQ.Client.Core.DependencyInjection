using System;
using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// Message handler router model.
    /// </summary>
    public class MessageHandlerRouter
    {
        public MessageHandlerRouter(Type type, string? exchange, IList<string> routePatterns)
        {
            Type = type;
            Exchange = exchange;
            RoutePatterns = routePatterns;
        }
        
        /// <summary>
        /// Message Handler Type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// An exchange which is being listened by the message handler by specified route patterns.
        /// </summary>
        /// <remarks>
        /// Exchange can be null, and in that scenario message handler will be general
        /// (it will listen all messages regardless of an exchange).
        /// </remarks>
        public string? Exchange { get; set; }
        
        /// <summary>
        /// Collection of route patterns (routing keys) that handler will be "listening".
        /// </summary>
        public IList<string> RoutePatterns { get; set; }

        /// <summary>
        /// Flag is the message handler general.
        /// </summary>
        public bool IsGeneral => string.IsNullOrEmpty(Exchange);
    }
}