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

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Реализация интерфейса кастомного клиента обмена сообщениями RabbitMQ.
    /// </summary>
    public class QueueService : IQueueService
    {
        /// <summary>
        /// Интерфейс соединения RabbitMQ.
        /// </summary>
        public IConnection Connection => _connection;

        /// <summary>
        /// Канал RabbitMQ.
        /// </summary>
        public IModel Channel => _channel;

        EventHandler<BasicDeliverEventArgs> _receivedMessage;
        bool _consumingStarted = false;

        readonly IDictionary<string, IList<IMessageHandler>> _messageHandlers;
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
            ILoggerFactory loggerFactory,
            IOptions<RabbitMqClientOptions> options,
            ILogger clientLogger = null)
        {
            if (options is null)
                throw new ArgumentException($"Argument {nameof(options)} is null.", nameof(options));

            _messageHandlers = TransformMessageHandlersCollection(messageHandlers);
            _exchanges = exchanges;
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

                // Настройки по умолчанию.
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                RequestedConnectionTimeout = 60000,
                RequestedHeartbeat = 60
            };

            _connection = factory.CreateConnection();
            // Обработка событий.
            _connection.CallbackException += HandleConnectionCallbackException;
            _connection.ConnectionRecoveryError += HandleConnectionRecoveryError;

            _channel = _connection.CreateModel();
            // Обработка событий.
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
        /// Начать "прослушивать" очереди (получать сообщения).
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
        /// Отправить сообщение.
        /// </summary>
        /// <typeparam name="T">Класс.</typeparam>
        /// <param name="object">Объект, отправляемый в качестве сообщения.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
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
        /// Отправить сообщение.
        /// </summary>
        /// <param name="json">Сообщение в формате json.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
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
        /// Отправить сообщение.
        /// </summary>
        /// <param name="bytes">Собщение в формате массива байт.</param>
        /// <param name="properties"></param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        public void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));

            if (string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));

            // BasicPublish операция не потокобезопасная.
            lock (_lock)
            {
                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        /// <summary>
        /// Асинхронно отправить сообщение.
        /// </summary>
        /// <typeparam name="T">Класс.</typeparam>
        /// <param name="object">Объект, отправляемый в качестве сообщения.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        /// <returns></returns>
        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey));

        /// <summary>
        /// Асинхронно отправить сообщение.
        /// </summary>
        /// <param name="json">Сообщение в формате json.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        /// <returns></returns>
        public async Task SendJsonAsync(string json, string exchangeName, string routingKey) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey));

        /// <summary>
        /// Асинхронно отправить сообщение.
        /// </summary>
        /// <param name="bytes">Собщение в формате массива байт.</param>
        /// <param name="properties"></param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
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

        IDictionary<string, IList<IMessageHandler>> TransformMessageHandlersCollection(IEnumerable<IMessageHandler> messageHandlers)
        {
            var dictionary = new Dictionary<string, IList<IMessageHandler>>();
            foreach (var handler in messageHandlers)
            {
                foreach (var routingKey in handler.RoutingKeys)
                {
                    if (dictionary.ContainsKey(routingKey))
                        dictionary[routingKey].Add(handler);
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

            // Добавить отложенные сообщения через dead-letter-exchanges.
            // Отправлять туда сообщения, которым был выдан "отказ" или не пришло подтверждение о получении.

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
                // Делаем привязку очереди и обменника по ключам маршрутизации.
                foreach (var route in queue.RoutingKeys)
                    _channel.QueueBind(
                        queue: queue.Name,
                        exchange: exchangeName,
                        routingKey: route);
            }
            else
            {
                // Делаем привязку обменника и очереди по наменованию очереди.
                // Наименование очереди и будет routing-key.
                _channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchangeName,
                    routingKey: queue.Name);
            }
        }
    }
}