using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc/>
    public class MessageHandlerContainerBuilder : IMessageHandlerContainerBuilder
    {
        private readonly IEnumerable<MessageHandlerRouter> _routers;
        private readonly IEnumerable<MessageHandlerOrderingModel> _orderingModels;
        private readonly IEnumerable<IMessageHandler> _messageHandlers;
        private readonly IEnumerable<IAsyncMessageHandler> _asyncMessageHandlers;

        public MessageHandlerContainerBuilder(
            IEnumerable<MessageHandlerRouter> routers,
            IEnumerable<MessageHandlerOrderingModel> orderingModels,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers)
        {
            _routers = routers;
            _orderingModels = orderingModels;
            _messageHandlers = messageHandlers;
            _asyncMessageHandlers = asyncMessageHandlers;
        }

        /// <inheritdoc/>
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

        private MessageHandlerContainer CreateContainer(string? exchange, IList<MessageHandlerRouter> selectedRouters)
        {
            var routersDictionary = TransformMessageHandlerRoutersToDictionary(selectedRouters);
            var boundMessageHandlers = _messageHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType())).ToList();
            var boundAsyncMessageHandlers = _asyncMessageHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType())).ToList();
            var routePatterns = selectedRouters.SelectMany(x => x.RoutePatterns).Distinct().ToList();
            var messageHandlers = TransformMessageHandlersCollectionsToDictionary(
                boundMessageHandlers,
                boundAsyncMessageHandlers,
                routersDictionary);
            var orderingModels = GetMessageHandlerOrderingModels(
                exchange,
                boundMessageHandlers,
                boundAsyncMessageHandlers,
                _orderingModels);
            return new MessageHandlerContainer(exchange, WildcardExtensions.ConstructRoutesTree(routePatterns), messageHandlers, orderingModels);
        }

        private static IDictionary<Type, IList<string>> TransformMessageHandlerRoutersToDictionary(IEnumerable<MessageHandlerRouter> routers)
        {
            var dictionary = new Dictionary<Type, IList<string>>();
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

        private static IEnumerable<MessageHandlerOrderingModel> GetMessageHandlerOrderingModels(
            string? exchange,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<MessageHandlerOrderingModel> orderingModels)
        {
            var messageHandlersCollection = new List<IBaseMessageHandler>(messageHandlers);
            messageHandlersCollection.AddRange(asyncMessageHandlers);

            var messageHandlerTypes = messageHandlersCollection.Select(x => x.GetType()).ToList();
            return orderingModels.Where(x => messageHandlerTypes.Contains(x.MessageHandlerType) && x.Exchange == exchange);
        }

        private static IDictionary<string, IList<IBaseMessageHandler>> TransformMessageHandlersCollectionsToDictionary(
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IDictionary<Type, IList<string>> routersDictionary)
        {
            var transformedMessageHandlers = TransformMessageHandlersCollectionToDictionary(messageHandlers, routersDictionary);
            var transformedAsyncMessageHandlers = TransformMessageHandlersCollectionToDictionary(asyncMessageHandlers, routersDictionary);
            return transformedMessageHandlers.UnionKeysAndValues(transformedAsyncMessageHandlers);
        }

        private static IDictionary<string, IList<IBaseMessageHandler>> TransformMessageHandlersCollectionToDictionary<T>(
            IEnumerable<T> messageHandlers,
            IDictionary<Type, IList<string>> routersDictionary)
            where T : class, IBaseMessageHandler
        {
            var dictionary = new Dictionary<string, IList<IBaseMessageHandler>>();
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
                        dictionary.Add(routingKey, new List<IBaseMessageHandler> { handler });
                    }
                }
            }
            return dictionary;
        }
    }
}