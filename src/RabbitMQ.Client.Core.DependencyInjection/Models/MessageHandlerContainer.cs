using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// The service model that represents a container that contains a route patterns tree connected with an exchange and all types of message handlers.
    /// If route patterns configured without an exchange they get in the "general" container.
    /// </summary>
    public class MessageHandlerContainer
    {
        /// <summary>
        /// An exchange.
        /// </summary>
        /// <remarks>
        /// Could be null.
        /// </remarks>
        public string Exchange { get; set; }

        /// <summary>
        /// Route patterns tree (trie) structure.
        /// </summary>
        public IEnumerable<TreeNode> Tree { get; set; }

        /// <summary>
        /// Flag is the container general.
        /// </summary>
        public bool IsGeneral => string.IsNullOrEmpty(Exchange);

        /// <summary>
        /// Dictionary of route patterns and message handlers connected by them.
        /// </summary>
        public IDictionary<string, IList<IMessageHandler>> MessageHandlers { get; set; }

        /// <summary>
        /// Dictionary of route patterns and async message handlers connected by them.
        /// </summary>
        public IDictionary<string, IList<IAsyncMessageHandler>> AsyncMessageHandlers { get; set; }

        /// <summary>
        /// Dictionary of route patterns and non-cyclic message handlers connected by them.
        /// </summary>
        public IDictionary<string, IList<INonCyclicMessageHandler>> NonCyclicHandlers { get; set; }

        /// <summary>
        /// Dictionary of route patterns and async non-cyclic message handlers connected by them.
        /// </summary>
        public IDictionary<string, IList<IAsyncNonCyclicMessageHandler>> AsyncNonCyclicHandlers { get; set; }
    }
}