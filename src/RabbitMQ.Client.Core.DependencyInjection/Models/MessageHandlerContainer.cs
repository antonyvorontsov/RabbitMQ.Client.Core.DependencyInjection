using System.Collections.Generic;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// The service model that represents a container that contains a route patterns tree connected with an exchange and all types of message handlers.
    /// If route patterns configured without an exchange they get in the "general" container.
    /// </summary>
    public class MessageHandlerContainer
    {
        public MessageHandlerContainer(string? exchange, IEnumerable<TreeNode> tree, IDictionary<string, IList<IBaseMessageHandler>> messageHandlers, IEnumerable<MessageHandlerOrderingModel> messageHandlerOrderingModels)
        {
            Exchange = exchange;
            Tree = tree;
            MessageHandlers = messageHandlers;
            MessageHandlerOrderingModels = messageHandlerOrderingModels;
        }
        
        /// <summary>
        /// An exchange.
        /// </summary>
        /// <remarks>
        /// Could be null.
        /// </remarks>
        public string? Exchange { get; }

        /// <summary>
        /// Route patterns tree (trie) structure.
        /// </summary>
        public IEnumerable<TreeNode> Tree { get; }

        /// <summary>
        /// Flag is the container general.
        /// </summary>
        // TODO: validate that everything is okay with general exchanges.
        public bool IsGeneral => string.IsNullOrEmpty(Exchange);

        /// <summary>
        /// Dictionary of route patterns and message handlers connected by them.
        /// </summary>
        public IDictionary<string, IList<IBaseMessageHandler>> MessageHandlers { get; }

        /// <summary>
        /// The collection of models that contain information about an order in which message handlers will be called.
        /// </summary>
        public IEnumerable<MessageHandlerOrderingModel> MessageHandlerOrderingModels { get; }
    }
}