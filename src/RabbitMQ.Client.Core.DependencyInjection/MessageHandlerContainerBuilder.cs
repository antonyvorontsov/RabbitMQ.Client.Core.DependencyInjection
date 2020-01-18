using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.Extensions;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Implementation of the service that build message handler containers collection.
    /// Those containers contain information about message handlers (all types) bound to the exchange.
    /// Container could be "general" if message handler has not been bound to the exchange (so it will "listen" regardless of the exchange).
    /// </summary>
    public class MessageHandlerContainerBuilder : IMessageHandlerContainerBuilder
    {
        readonly IEnumerable<MessageHandlerRouter> _routers;
        readonly IEnumerable<IMessageHandler> _messageHandlers;
        readonly IEnumerable<IAsyncMessageHandler> _asyncMessageHandlers;
        readonly IEnumerable<INonCyclicMessageHandler> _nonCyclicHandlers;
        readonly IEnumerable<IAsyncNonCyclicMessageHandler> _asyncNonCyclicHandlers;

        public MessageHandlerContainerBuilder(
            IEnumerable<MessageHandlerRouter> routers,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<INonCyclicMessageHandler> nonCyclicHandlers,
            IEnumerable<IAsyncNonCyclicMessageHandler> asyncNonCyclicHandlers)
        {
            _routers = routers;
            _messageHandlers = messageHandlers;
            _asyncMessageHandlers = asyncMessageHandlers;
            _nonCyclicHandlers = nonCyclicHandlers;
            _asyncNonCyclicHandlers = asyncNonCyclicHandlers;
        }

        /// <summary>
        /// Build message handler containers collection.
        /// </summary>
        /// <returns>Collection of message handler containers <see cref="MessageHandlerContainer"/>.</returns>
        public IEnumerable<MessageHandlerContainer> BuildCollection()
        {
            var containers = new List<MessageHandlerContainer>();
            var generalRouters = _routers.Where(x => x.IsGeneral).ToList();
            if (generalRouters.Any())
            {
                var container = CreateContainer(null, generalRouters);
                containers.Add(container);
            }

            var exchanges = _routers.Where(x => !x.IsGeneral).Select(x => x.Exchange).Distinct().ToList();
            foreach (var exchange in exchanges)
            {
                var exchangeRouters = _routers.Where(x => x.Exchange == exchange).ToList();
                var container = CreateContainer(exchange, exchangeRouters);
                containers.Add(container);
            }
            return containers;
        }

        MessageHandlerContainer CreateContainer(string exchange, IEnumerable<MessageHandlerRouter> selectedRouters)
        {
            var routersDictionary = TransformMessageHandlerRoutersToDictionary(selectedRouters);
            var boundMessageHandlers = _messageHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
            var boundAsyncMessageHandlers = _asyncMessageHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
            var boundNonCyclicMessageHandlers = _nonCyclicHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
            var boundAsyncNonCyclicMessageHandlers = _asyncNonCyclicHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
            var routePatterns = selectedRouters.SelectMany(x => x.RoutePatterns).Distinct().ToList();
            return new MessageHandlerContainer
            {
                Exchange = exchange,
                Tree = WildcardExtensions.ConstructRoutesTree(routePatterns),
                MessageHandlers = TransformMessageHandlersCollectionToDictionary(boundMessageHandlers, routersDictionary),
                AsyncMessageHandlers = TransformMessageHandlersCollectionToDictionary(boundAsyncMessageHandlers, routersDictionary),
                NonCyclicHandlers = TransformMessageHandlersCollectionToDictionary(boundNonCyclicMessageHandlers, routersDictionary),
                AsyncNonCyclicHandlers = TransformMessageHandlersCollectionToDictionary(boundAsyncNonCyclicMessageHandlers, routersDictionary)
            };
        }

        static IDictionary<Type, List<string>> TransformMessageHandlerRoutersToDictionary(IEnumerable<MessageHandlerRouter> routers)
        {
            var dictionary = new Dictionary<Type, List<string>>();
            foreach (var router in routers)
            {
                if (dictionary.ContainsKey(router.Type))
                {
                    dictionary[router.Type] = dictionary[router.Type].Union(router.RoutePatterns).ToList();
                }
                else
                {
                    dictionary.Add(router.Type, router.RoutePatterns);
                }
            }
            return dictionary;
        }

        static IDictionary<string, IList<T>> TransformMessageHandlersCollectionToDictionary<T>(
            IEnumerable<T> messageHandlers,
            IDictionary<Type, List<string>> routersDictionary)
        {
            var dictionary = new Dictionary<string, IList<T>>();
            foreach (var handler in messageHandlers)
            {
                var type = handler.GetType();
                foreach (var routingKey in routersDictionary[type])
                {
                    if (dictionary.ContainsKey(routingKey))
                    {
                        if (!dictionary[routingKey].Any(x => x.GetType() == handler.GetType()))
                        {
                            dictionary[routingKey].Add(handler);
                        }
                    }
                    else
                    {
                        dictionary.Add(routingKey, new List<T> { handler });
                    }
                }
            }
            return dictionary;
        }
    }
}