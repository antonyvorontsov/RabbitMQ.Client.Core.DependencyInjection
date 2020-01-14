using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Extensions;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// An implementation of the service that handles message receiving (consumption) events.
    /// </summary>
    public class MessageHandlingService : IMessageHandlingService
    {
        const int ResendTimeout = 60;
        
        readonly IEnumerable<RabbitMqExchange> _exchanges;
        readonly IEnumerable<MessageHandlerContainer> _messageHandlerContainers;
        readonly ILogger<MessageHandlingService> _logger;
        
        public MessageHandlingService(
            IEnumerable<RabbitMqExchange> exchanges,
            IEnumerable<MessageHandlerRouter> routers,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<INonCyclicMessageHandler> nonCyclicHandlers,
            IEnumerable<IAsyncNonCyclicMessageHandler> asyncNonCyclicHandlers,
            ILogger<MessageHandlingService> logger)
        {
            _exchanges = exchanges;
            _messageHandlerContainers = ConstructMessageHandlerContainers(
                routers,
                messageHandlers,
                asyncMessageHandlers,
                nonCyclicHandlers,
                asyncNonCyclicHandlers);
            _logger = logger;
        }
        
        /// <summary>
        /// Handle message receiving event.
        /// </summary>
        /// <param name="eventArgs">Arguments of message receiving event.</param>
        /// <param name="queueService">An instance of the queue service <see cref="IQueueService"/>.</param>
        public void HandleMessageReceivingEvent(BasicDeliverEventArgs eventArgs, IQueueService queueService)
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body);

            _logger.LogInformation($"A new message was received with deliveryTag {eventArgs.DeliveryTag}.");
            _logger.LogInformation(message);

            try
            {
                var matchingRoutes = GetMatchingRoutePatterns(eventArgs.Exchange, eventArgs.RoutingKey);
                ProcessMessage(eventArgs.Exchange, message, queueService, matchingRoutes);
                queueService.Channel.BasicAck(eventArgs.DeliveryTag, false);
                _logger.LogInformation($"Message processing finished successfully. Acknowledge has been sent with deliveryTag {eventArgs.DeliveryTag}.");
            }
            catch (Exception exception)
            {
                _logger.LogError(new EventId(), exception, $"An error occurred while processing received message with the delivery tag {eventArgs.DeliveryTag}.");

                queueService.Channel.BasicAck(eventArgs.DeliveryTag, false);

                if (eventArgs.BasicProperties.Headers is null)
                {
                    eventArgs.BasicProperties.Headers = new Dictionary<string, object>();
                }

                var exchange = _exchanges.FirstOrDefault(x => x.Name == eventArgs.Exchange);
                if (exchange is null)
                {
                    _logger.LogError($"Could not detect an exchange \"{eventArgs.Exchange}\" to determine the necessity of resending the failed message.");
                    return;
                }

                if (exchange.Options.RequeueFailedMessages
                    && !string.IsNullOrEmpty(exchange.Options.DeadLetterExchange)
                    && !eventArgs.BasicProperties.Headers.ContainsKey("requeued"))
                {
                    eventArgs.BasicProperties.Headers.Add("requeued", true);
                    queueService.Send(eventArgs.Body, eventArgs.BasicProperties, eventArgs.Exchange, eventArgs.RoutingKey, ResendTimeout);
                    _logger.LogInformation("The failed message has been requeued.");
                }
                else
                {
                    _logger.LogInformation("The failed message would not be requeued.");
                }
            }
        }

        IEnumerable<string> GetMatchingRoutePatterns(string exchange, string routingKey)
        {
            var tree = _messageHandlerContainers.FirstOrDefault(x => x.Exchange == exchange)?.Tree 
                ?? _messageHandlerContainers.FirstOrDefault(x => x.IsGeneral)?.Tree;
            if (tree is null)
            {
                return Enumerable.Empty<string>();
            }

            var routingKeyParts = routingKey.Split(".");
            return WildcardExtensions.GetMatchingRoutePatterns(tree, routingKeyParts).ToList();
        }

        void ProcessMessage(string exchange, string message, IQueueService queueService, IEnumerable<string> matchingRoutes)
        {
            var container = _messageHandlerContainers.FirstOrDefault(x => x.Exchange == exchange) ?? _messageHandlerContainers.FirstOrDefault(x => x.IsGeneral);
            if (container is null)
            {
                return;
            }

            var executedHandlers = new List<Type>();
            foreach (var matchingRoute in matchingRoutes)
            {
                if (container.AsyncMessageHandlers.ContainsKey(matchingRoute))
                {
                    var tasks = new List<Task>();
                    foreach (var handler in container.AsyncMessageHandlers[matchingRoute])
                    {
                        var handlerType = handler.GetType();
                        if (executedHandlers.Contains(handlerType))
                        {
                            continue;
                        }

                        executedHandlers.Add(handlerType);
                        tasks.Add(RunAsyncMessageHandler(handler, message, matchingRoute));
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                if (container.MessageHandlers.ContainsKey(matchingRoute))
                {
                    foreach (var handler in container.MessageHandlers[matchingRoute])
                    {
                        var handlerType = handler.GetType();
                        if (executedHandlers.Contains(handlerType))
                        {
                            continue;
                        }

                        executedHandlers.Add(handlerType);
                        RunMessageHandler(handler, message, matchingRoute);
                    }
                }
                if (container.AsyncNonCyclicHandlers.ContainsKey(matchingRoute))
                {
                    var tasks = new List<Task>();
                    foreach (var handler in container.AsyncNonCyclicHandlers[matchingRoute])
                    {
                        var handlerType = handler.GetType();
                        if (executedHandlers.Contains(handlerType))
                        {
                            continue;
                        }

                        executedHandlers.Add(handlerType);
                        tasks.Add(RunAsyncNonCyclicMessageHandler(handler, message, matchingRoute, queueService));
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                if (container.NonCyclicHandlers.ContainsKey(matchingRoute))
                {
                    foreach (var handler in container.NonCyclicHandlers[matchingRoute])
                    {
                        var handlerType = handler.GetType();
                        if (executedHandlers.Contains(handlerType))
                        {
                            continue;
                        }

                        executedHandlers.Add(handlerType);
                        RunNonCyclicMessageHandler(handler, message, matchingRoute, queueService);
                    }
                } 
            }
        }

        void RunMessageHandler(IMessageHandler handler, string message, string routingKey)
        {
            ValidateHandler(handler);
            _logger.LogDebug($"Starting processing the message by message handler {handler.GetType().Name}.");
            handler.Handle(message, routingKey);
            _logger.LogDebug($"The message has been processed by message handler {handler.GetType().Name}.");
        }

        void RunNonCyclicMessageHandler(INonCyclicMessageHandler handler, string message, string routingKey, IQueueService queueService)
        {
            ValidateHandler(handler);
            _logger.LogDebug($"Starting processing the message by non-cyclic message handler {handler.GetType().Name}.");
            handler?.Handle(message, routingKey, queueService);
            _logger.LogDebug($"The message has been processed by non-cyclic message handler {handler.GetType().Name}.");
        }

        async Task RunAsyncMessageHandler(IAsyncMessageHandler handler, string message, string routingKey)
        {
            ValidateHandler(handler);
            _logger.LogDebug($"Starting processing the message by async message handler {handler.GetType().Name}.");
            await handler.Handle(message, routingKey);
            _logger.LogDebug($"The message has been processed by async message handler {handler.GetType().Name}.");
        }

        async Task RunAsyncNonCyclicMessageHandler(IAsyncNonCyclicMessageHandler handler, string message, string routingKey, IQueueService queueService)
        {
            ValidateHandler(handler);
            _logger.LogDebug($"Starting processing the message by async non-cyclic message handler {handler.GetType().Name}.");
            await handler.Handle(message, routingKey, queueService);
            _logger.LogDebug($"The message has been processed by async non-cyclic message handler {handler.GetType().Name}.");
        }

        static void ValidateHandler<T>(T messageHandler)
        {
            if (messageHandler is null)
            {
                throw new ArgumentNullException(nameof(messageHandler), "Message handler is null.");
            }
        }

        static IEnumerable<MessageHandlerContainer> ConstructMessageHandlerContainers(
            IEnumerable<MessageHandlerRouter> routers,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<INonCyclicMessageHandler> nonCyclicHandlers,
            IEnumerable<IAsyncNonCyclicMessageHandler> asyncNonCyclicHandlers)
        {
            var containers = new List<MessageHandlerContainer>();
            var generalRouters = routers.Where(x => x.IsGeneral).ToList();
            if (generalRouters.Any())
            {
                var container = CreateContailer(
                    null,
                    generalRouters,
                    messageHandlers,
                    asyncMessageHandlers,
                    nonCyclicHandlers,
                    asyncNonCyclicHandlers);
                containers.Add(container);
            }
            
            var exchanges = routers.Where(x => !x.IsGeneral).Select(x => x.Exchange).Distinct().ToList();
            foreach (var exchange in exchanges)
            {
                var exchangeRouters = routers.Where(x => x.Exchange == exchange).ToList();
                var container = CreateContailer(
                    exchange,
                    exchangeRouters,
                    messageHandlers,
                    asyncMessageHandlers,
                    nonCyclicHandlers,
                    asyncNonCyclicHandlers);
                containers.Add(container);
            }
            return containers;
        }

        static MessageHandlerContainer CreateContailer(
            string exchange,
            IEnumerable<MessageHandlerRouter> selectedRouters,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<INonCyclicMessageHandler> nonCyclicHandlers,
            IEnumerable<IAsyncNonCyclicMessageHandler> asyncNonCyclicHandlers)
        {
            var routersDictionary = TransformMessageHandlerRoutersToDictionary(selectedRouters);
            var boundMessageHandlers = messageHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
            var boundAsyncMessageHandlers = asyncMessageHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
            var boundNonCyclicMessageHandlers = nonCyclicHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
            var boundAsyncNonCyclicMessageHandlers = asyncNonCyclicHandlers.Where(x => routersDictionary.Keys.Contains(x.GetType()));
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