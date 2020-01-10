using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// An implementation of the service that handles message consumption events.
    /// </summary>
    public class MessageHandlingService : IMessageHandlingService
    {
        const int ResendTimeout = 60;
        
        readonly IEnumerable<RabbitMqExchange> _exchanges;
        readonly IDictionary<Type, List<string>> _messageHandlerRouters;
        readonly IDictionary<string, IList<IMessageHandler>> _messageHandlers;
        readonly IDictionary<string, IList<IAsyncMessageHandler>> _asyncMessageHandlers;
        readonly IDictionary<string, IList<INonCyclicMessageHandler>> _nonCyclicHandlers;
        readonly IDictionary<string, IList<IAsyncNonCyclicMessageHandler>> _asyncNonCyclicHandlers;
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
            _messageHandlerRouters = TransformMessageHandlerRouters(routers);
            _messageHandlers = TransformMessageHandlersCollection(messageHandlers);
            _asyncMessageHandlers = TransformMessageHandlersCollection(asyncMessageHandlers);
            _nonCyclicHandlers = TransformMessageHandlersCollection(nonCyclicHandlers);
            _asyncNonCyclicHandlers = TransformMessageHandlersCollection(asyncNonCyclicHandlers);
            _logger = logger;
        }
        
        /// <summary>
        /// Handle message consumption event.
        /// </summary>
        /// <param name="eventArgs">Arguments of message consumption event.</param>
        /// <param name="queueService">An instance of the queue service <see cref="IQueueService"/>.</param>
        public void HandleMessage(BasicDeliverEventArgs eventArgs, IQueueService queueService)
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body);

                _logger.LogInformation($"New message was received with deliveryTag {eventArgs.DeliveryTag}.");
                _logger.LogInformation(message);

                try
                {
                    if (_asyncMessageHandlers.ContainsKey(eventArgs.RoutingKey))
                    {
                        var tasks = new List<Task>();
                        foreach (var handler in _asyncMessageHandlers[eventArgs.RoutingKey])
                        {
                            tasks.Add(RunAsyncHandler(handler, message, eventArgs.RoutingKey));
                        }
                        Task.WaitAll(tasks.ToArray());
                    }
                    if (_messageHandlers.ContainsKey(eventArgs.RoutingKey))
                    {
                        foreach (var handler in _messageHandlers[eventArgs.RoutingKey])
                        {
                            _logger.LogDebug($"Starting processing the message by message handler {handler?.GetType().Name}.");
                            handler.Handle(message, eventArgs.RoutingKey);
                            _logger.LogDebug($"The message has been processed by message handler {handler?.GetType().Name}.");
                        }
                    }
                    if (_asyncNonCyclicHandlers.ContainsKey(eventArgs.RoutingKey))
                    {
                        var tasks = new List<Task>();
                        foreach (var handler in _asyncNonCyclicHandlers[eventArgs.RoutingKey])
                        {
                            tasks.Add(RunAsyncNonCyclicHandler(handler, message, eventArgs.RoutingKey, queueService));
                        }
                        Task.WaitAll(tasks.ToArray());
                    }
                    if (_nonCyclicHandlers.ContainsKey(eventArgs.RoutingKey))
                    {
                        foreach (var handler in _nonCyclicHandlers[eventArgs.RoutingKey])
                        {
                            _logger.LogDebug($"Starting processing the message by non-cyclic message handler {handler?.GetType().Name}.");
                            handler.Handle(message, eventArgs.RoutingKey, queueService);
                            _logger.LogDebug($"The message has been processed by non-cyclic message handler {handler?.GetType().Name}.");
                        }
                    }
                    _logger.LogInformation($"Success message with deliveryTag {eventArgs.DeliveryTag}.");
                    queueService.Channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception exception)
                {
                    _logger.LogError(new EventId(), exception, $"An error occurred while processing received message with delivery tag {eventArgs.DeliveryTag}.");

                    queueService.Channel.BasicAck(eventArgs.DeliveryTag, false);

                    if (eventArgs.BasicProperties.Headers is null)
                    {
                        eventArgs.BasicProperties.Headers = new Dictionary<string, object>();
                    }

                    var exchange = _exchanges.FirstOrDefault(x => x.Name == eventArgs.Exchange);
                    if (exchange is null)
                    {
                        _logger.LogError($"Could not detect exchange {eventArgs.Exchange} to detect the necessity of resending the failed message.");
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

        async Task RunAsyncHandler(IAsyncMessageHandler handler, string message, string routingKey)
        {
            _logger.LogDebug($"Starting processing the message by async message handler {handler?.GetType().Name}.");
            await handler.Handle(message, routingKey);
            _logger.LogDebug($"The message has been processed by async message handler {handler?.GetType().Name}.");
        }

        async Task RunAsyncNonCyclicHandler(IAsyncNonCyclicMessageHandler handler, string message, string routingKey, IQueueService queueService)
        {
            _logger.LogDebug($"Starting processing the message by async non-cyclic message handler {handler?.GetType().Name}.");
            await handler.Handle(message, routingKey, queueService);
            _logger.LogDebug($"The message has been processed by async non-cyclic message handler {handler?.GetType().Name}.");
        }

        IDictionary<Type, List<string>> TransformMessageHandlerRouters(IEnumerable<MessageHandlerRouter> routers)
        {
            var dictionary = new Dictionary<Type, List<string>>();
            foreach (var router in routers)
            {
                if (dictionary.ContainsKey(router.Type))
                {
                    dictionary[router.Type] = dictionary[router.Type].Union(router.RoutingKeys).ToList();
                }
                else
                {
                    dictionary.Add(router.Type, router.RoutingKeys);
                }
            }
            return dictionary;
        }
        
        IDictionary<string, IList<T>> TransformMessageHandlersCollection<T>(IEnumerable<T> messageHandlers)
        {
            var dictionary = new Dictionary<string, IList<T>>();
            foreach (var handler in messageHandlers)
            {
                var type = handler.GetType();
                foreach (var routingKey in _messageHandlerRouters[type])
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