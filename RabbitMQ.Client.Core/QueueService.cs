using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Implementation of the custom RabbitMQ queue service.
    /// </summary>
    public class QueueService : IQueueService
    {
        /// <summary>
        /// RabbitMQ connection.
        /// </summary>
        public IConnection Connection => _connection;

        /// <summary>
        /// RabbitMQ channel.
        /// </summary>
        public IModel Channel => _channel;

        EventHandler<BasicDeliverEventArgs> _receivedMessage;
        bool _consumingStarted = false;

        readonly IDictionary<string, IList<IMessageHandler>> _messageHandlers;
        readonly IDictionary<Type, List<string>> _routingKeys;
        readonly IEnumerable<RabbitMqExchange> _exchanges;
        readonly ILogger<QueueService> _logger;
        readonly ILogger _clientLogger;
        readonly IConnection _connection;
        readonly IModel _channel;
        readonly EventingBasicConsumer _consumer;
        readonly object _lock = new object();

        public QueueService(
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<RabbitMqExchange> exchanges,
            IEnumerable<MessageHandlerRouter> routers,
            ILoggerFactory loggerFactory,
            IOptions<RabbitMqClientOptions> options,
            ILogger clientLogger = null)
        {
            if (options is null)
                throw new ArgumentException($"Argument {nameof(options)} is null.", nameof(options));

            _exchanges = exchanges;
            _routingKeys = TransformMessageHandlerRouters(routers);
            _messageHandlers = TransformMessageHandlersCollection(messageHandlers);
            _logger = loggerFactory.CreateLogger<QueueService>();
            _clientLogger = clientLogger;

            var optionsValue = options.Value;
            var factory = new ConnectionFactory
            {
                HostName = optionsValue.HostName,
                Port = optionsValue.Port,
                UserName = optionsValue.UserName,
                Password = optionsValue.Password,
                VirtualHost = optionsValue.VirtualHost,

                // Default settings.
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                RequestedConnectionTimeout = 60000,
                RequestedHeartbeat = 60
            };

            _connection = factory.CreateConnection();
            // Event handling.
            _connection.CallbackException += HandleConnectionCallbackException;
            _connection.ConnectionRecoveryError += HandleConnectionRecoveryError;

            _channel = _connection.CreateModel();
            // Event handling.
            _channel.CallbackException += HandleChannelCallbackException;
            _channel.BasicRecoverOk += HandleChannelBasicRecoverOk;

            _consumer = new EventingBasicConsumer(_channel);
            StartClient();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.CallbackException -= HandleConnectionCallbackException;
                _connection.ConnectionRecoveryError -= HandleConnectionRecoveryError;
            }
            if (_channel != null)
            {
                _channel.CallbackException -= HandleChannelCallbackException;
                _channel.BasicRecoverOk -= HandleChannelBasicRecoverOk;
            }

            if (_channel?.IsOpen == true)
                _channel.Close((int)HttpStatusCode.OK, "Channel closed");

            if (_connection?.IsOpen == true)
                _connection.Close();
        }

        /// <summary>
        /// Start comsuming (getting messages).
        /// </summary>
        public void StartConsuming()
        {
            if (_consumingStarted)
                return;

            _consumer.Received += _receivedMessage;
            _consumingStarted = true;

            foreach (var exchange in _exchanges)
                foreach (var queue in exchange.Options.Queues)
                    _channel.BasicConsume(queue: queue.Name, autoAck: false, consumer: _consumer);
        }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        public void Send<T>(T @object, string exchangeName, string routingKey) where T : class
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));

            if (string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));

            var json = JsonConvert.SerializeObject(@object);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        public void SendJson(string json, string exchangeName, string routingKey)
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));

            if (string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));

            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        public void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));

            if (string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));

            // BasicPublish is not thread-safe.
            lock (_lock)
            {
                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey));

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        public async Task SendJsonAsync(string json, string exchangeName, string routingKey) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey));

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        public async Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey));

        IBasicProperties CreateJsonProperties()
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            return properties;
        }

        void HandleConnectionCallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
                return;
            
            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        void HandleConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs @event)
        {
            if (@event is null)
                return;

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        void HandleChannelBasicRecoverOk(object sender, EventArgs @event)
        {
            if (@event is null)
                return;
            _logger.LogInformation("Connection has been reestablished.");
        }

        void HandleChannelCallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
                return;
            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
        }

        IDictionary<Type, List<string>> TransformMessageHandlerRouters(IEnumerable<MessageHandlerRouter> routers)
        {
            var dictionary = new Dictionary<Type, List<string>>();
            foreach (var router in routers)
            {
                if (dictionary.ContainsKey(router.Type))
                    dictionary[router.Type] = dictionary[router.Type].Union(router.RoutingKeys).ToList();
                else
                    dictionary.Add(router.Type, router.RoutingKeys);
            }
            return dictionary;
        }

        IDictionary<string, IList<IMessageHandler>> TransformMessageHandlersCollection(IEnumerable<IMessageHandler> messageHandlers)
        {
            var dictionary = new Dictionary<string, IList<IMessageHandler>>();
            foreach (var handler in messageHandlers)
            {
                var type = handler.GetType();
                foreach (var routingKey in _routingKeys[type])
                {
                    if (dictionary.ContainsKey(routingKey))
                    {
                        if (!dictionary[routingKey].Any(x => x.GetType() == handler.GetType()))
                            dictionary[routingKey].Add(handler);
                    }
                    else
                        dictionary.Add(routingKey, new List<IMessageHandler>() { handler });
                }
            }
            return dictionary;
        }

        void StartClient()
        {
            var logMessage = string.Empty;

            _receivedMessage = (sender, @event) =>
            {
                var message = Encoding.UTF8.GetString(@event.Body);

                logMessage = $"New message was received with deliveryTag {@event.DeliveryTag}.";
                _logger.LogInformation(logMessage);
                _logger.LogInformation(message);
                Log(LogLevel.Information, logMessage);
                Log(LogLevel.Information, message);

                try
                {
                    if (_messageHandlers.ContainsKey(@event.RoutingKey))
                    {
                        foreach (var handler in _messageHandlers[@event.RoutingKey])
                        {
                            logMessage = $"Starting processing the message by message handler {handler?.GetType().Name}.";
                            _logger.LogDebug(logMessage);
                            Log(LogLevel.Debug, logMessage);

                            handler.Handle(message, @event.RoutingKey);

                            logMessage = $"The message has been processed by message handler {handler?.GetType().Name}.";
                            _logger.LogDebug(logMessage);
                            Log(LogLevel.Debug, logMessage);
                        }
                    }
                    logMessage = $"Success message with deliveryTag {@event.DeliveryTag}.";
                    _logger.LogInformation(logMessage);
                    Log(LogLevel.Information, logMessage);

                    Channel.BasicAck(@event.DeliveryTag, false);
                }
                catch (Exception exception)
                {
                    logMessage = $"Error sending message with delivery tag {@event.DeliveryTag}.";
                    _logger.LogError(new EventId(), exception, logMessage);
                    Log(LogLevel.Error, logMessage);
                }
            };

            foreach (var exchange in _exchanges)
                StartExchange(exchange);
        }

        void Log(LogLevel logLevel, string message)
        {
            if (_clientLogger is null)
                return;
            _clientLogger.Log(logLevel, message);
        }

        void StartExchange(RabbitMqExchange exchange)
        {
            _channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Options.Type,
                durable: exchange.Options.Durable,
                autoDelete: exchange.Options.AutoDelete,
                arguments: exchange.Options.Arguments);

            // TODO: Add dead-letter-exchanges functionality.

            foreach (var queue in exchange.Options.Queues)
                StartQueue(queue, exchange.Name);
        }

        void StartQueue(RabbitMqQueueOptions queue, string exchangeName)
        {
            _channel.QueueDeclare(queue: queue.Name,
                    durable: queue.Durable,
                    exclusive: queue.Exclusive,
                    autoDelete: queue.AutoDelete,
                    arguments: queue.Arguments);

            if (queue.RoutingKeys.Count > 0)
            {
                // If there are not any routing keys then make a bind with a queue name.
                foreach (var route in queue.RoutingKeys)
                    _channel.QueueBind(
                        queue: queue.Name,
                        exchange: exchangeName,
                        routingKey: route);
            }
            else
            {
                _channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchangeName,
                    routingKey: queue.Name);
            }
        }
    }
}