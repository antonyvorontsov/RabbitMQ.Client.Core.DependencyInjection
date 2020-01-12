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
    /// An implementation of the service that handles message receiving (consumption) events.
    /// </summary>
    public class MessageHandlingService : IMessageHandlingService
    {
        const int ResendTimeout = 60;
        
        readonly IEnumerable<RabbitMqExchange> _exchanges;
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
            var messageHandlerRouters = TransformMessageHandlerRouters(routers);
            _messageHandlers = TransformMessageHandlersCollection(messageHandlers, messageHandlerRouters);
            _asyncMessageHandlers = TransformMessageHandlersCollection(asyncMessageHandlers, messageHandlerRouters);
            _nonCyclicHandlers = TransformMessageHandlersCollection(nonCyclicHandlers, messageHandlerRouters);
            _asyncNonCyclicHandlers = TransformMessageHandlersCollection(asyncNonCyclicHandlers, messageHandlerRouters);
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
                if (_asyncMessageHandlers.ContainsKey(eventArgs.RoutingKey))
                {
                    var tasks = new List<Task>();
                    foreach (var handler in _asyncMessageHandlers[eventArgs.RoutingKey])
                    {
                        tasks.Add(RunAsyncMessageHandler(handler, message, eventArgs.RoutingKey));
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                if (_messageHandlers.ContainsKey(eventArgs.RoutingKey))
                {
                    foreach (var handler in _messageHandlers[eventArgs.RoutingKey])
                    {
                        RunMessageHandler(handler, message, eventArgs.RoutingKey);
                    }
                }
                if (_asyncNonCyclicHandlers.ContainsKey(eventArgs.RoutingKey))
                {
                    var tasks = new List<Task>();
                    foreach (var handler in _asyncNonCyclicHandlers[eventArgs.RoutingKey])
                    {
                        tasks.Add(RunAsyncNonCyclicMessageHandler(handler, message, eventArgs.RoutingKey, queueService));
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                if (_nonCyclicHandlers.ContainsKey(eventArgs.RoutingKey))
                {
                    foreach (var handler in _nonCyclicHandlers[eventArgs.RoutingKey])
                    {
                        RunNonCyclicMessageHandler(handler, message, eventArgs.RoutingKey, queueService);
                    }
                }
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

        void ValidateHandler<T>(T messageHandler)
        {
            if (messageHandler is null)
            {
                throw new ArgumentNullException(nameof(messageHandler), "Message handler is null.");
            }
        }

        static IDictionary<Type, List<string>> TransformMessageHandlerRouters(IEnumerable<MessageHandlerRouter> routers)
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

        static IDictionary<string, IList<T>> TransformMessageHandlersCollection<T>(IEnumerable<T> messageHandlers, IDictionary<Type, List<string>> messageHandlerRouters)
        {
            var dictionary = new Dictionary<string, IList<T>>();
            foreach (var handler in messageHandlers)
            {
                var type = handler.GetType();
                foreach (var routingKey in messageHandlerRouters[type])
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