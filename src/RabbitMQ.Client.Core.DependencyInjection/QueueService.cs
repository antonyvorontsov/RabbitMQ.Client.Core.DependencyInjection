using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Implementation of the custom RabbitMQ queue service.
    /// </summary>
    internal class QueueService : IQueueService, IDisposable
    {
        public IConnection Connection => _connection;

        public IModel Channel => _channel;

        readonly IConnection _connection;
        readonly IModel _channel;
        
        readonly IMessageHandlingService _messageHandlingService;
        readonly IEnumerable<RabbitMqExchange> _exchanges;
        readonly ILogger<QueueService> _logger;
        readonly EventingBasicConsumer _consumer;

        EventHandler<BasicDeliverEventArgs> _receivedMessage;
        bool _consumingStarted;
        readonly object _lock = new object();

        const int QueueExpirationTime = 60000;

        public QueueService(
            IMessageHandlingService messageHandlingService,
            IEnumerable<RabbitMqExchange> exchanges,
            IOptions<RabbitMqClientOptions> options,
            ILogger<QueueService> logger)
        {
            if (options is null)
            {
                throw new ArgumentException($"Argument {nameof(options)} is null.", nameof(options));
            }

            _messageHandlingService = messageHandlingService;
            _exchanges = exchanges;
            _logger = logger;
            
            _connection = CreateRabbitMqConnection(options.Value);
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
            {
                _channel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (_connection?.IsOpen == true)
            {
                _connection.Close();
            }

            _channel?.Dispose();
            _connection?.Dispose();
        }

        public void StartConsuming()
        {
            if (_consumingStarted)
            {
                return;
            }

            _consumer.Received += _receivedMessage;
            _consumingStarted = true;

            var consumptionExchanges = _exchanges.Where(x => x.IsConsuming);
            foreach (var exchange in consumptionExchanges)
            {
                foreach (var queue in exchange.Options.Queues)
                {
                    _channel.BasicConsume(queue: queue.Name, autoAck: false, consumer: _consumer);
                }
            }
        }

        public void Send<T>(T @object, string exchangeName, string routingKey) where T : class
        {
            ValidateArguments(exchangeName, routingKey);
            var json = JsonConvert.SerializeObject(@object);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        public void Send<T>(T @object, string exchangeName, string routingKey, int secondsDelay) where T : class
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            Send(@object, deadLetterExchange, delayedQueueName);
        }

        public void SendJson(string json, string exchangeName, string routingKey)
        {
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        public void SendJson(string json, string exchangeName, string routingKey, int secondsDelay)
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            SendJson(json, deadLetterExchange, delayedQueueName);
        }

        public void SendString(string message, string exchangeName, string routingKey)
        {
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(message);
            Send(bytes, CreateProperties(), exchangeName, routingKey);
        }

        public void SendString(string message, string exchangeName, string routingKey, int secondsDelay)
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            SendString(message, deadLetterExchange, delayedQueueName);
        }

        public void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            ValidateArguments(exchangeName, routingKey);
            lock (_lock)
            {
                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        public void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay)
        {
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            Send(bytes, properties, deadLetterExchange, delayedQueueName);
        }

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey, int secondsDelay) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        public async Task SendJsonAsync(string json, string exchangeName, string routingKey) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendJsonAsync(string json, string exchangeName, string routingKey, int secondsDelay) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        public async Task SendStringAsync(string message, string exchangeName, string routingKey) =>
            await Task.Run(() => SendString(message, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendStringAsync(string message, string exchangeName, string routingKey, int secondsDelay) =>
            await Task.Run(() => SendString(message, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        public async Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        IBasicProperties CreateProperties()
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }

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
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        void HandleConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
            throw @event.Exception;
        }

        void HandleChannelBasicRecoverOk(object sender, EventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogInformation("Connection has been reestablished.");
        }

        void HandleChannelCallbackException(object sender, CallbackExceptionEventArgs @event)
        {
            if (@event is null)
            {
                return;
            }

            _logger.LogError(new EventId(), @event.Exception, @event.Exception.Message, @event);
        }

        void StartClient()
        {
            _receivedMessage = (sender, eventArgs) => _messageHandlingService.HandleMessageReceivingEvent(eventArgs, this);

            var deadLetterExchanges = _exchanges
                .Where(x => !string.IsNullOrEmpty(x.Options.DeadLetterExchange))
                .Select(x => x.Options.DeadLetterExchange)
                .Distinct();

            foreach (var exchangeName in deadLetterExchanges)
            {
                StartDeadLetterExchange(exchangeName);
            }

            foreach (var exchange in _exchanges)
            {
                StartExchange(exchange);
            }
        }

        void StartDeadLetterExchange(string exchangeName) =>
            _channel.ExchangeDeclare(
                exchange: exchangeName,
                type: "direct",
                durable: true,
                autoDelete: false,
                arguments: null);

        void StartExchange(RabbitMqExchange exchange)
        {
            _channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Options.Type,
                durable: exchange.Options.Durable,
                autoDelete: exchange.Options.AutoDelete,
                arguments: exchange.Options.Arguments);

            foreach (var queue in exchange.Options.Queues)
            {
                StartQueue(queue, exchange.Name);
            }
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
                foreach (var route in queue.RoutingKeys)
                {
                    _channel.QueueBind(
                        queue: queue.Name,
                        exchange: exchangeName,
                        routingKey: route);
                }
            }
            else
            {
                // If there are not any routing keys then make a bind with a queue name.
                _channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchangeName,
                    routingKey: queue.Name);
            }
        }

        void ValidateArguments(string exchangeName, string routingKey)
        {
            if (string.IsNullOrEmpty(exchangeName))
            {
                throw new ArgumentException($"Argument {nameof(exchangeName)} is null or empty.", nameof(exchangeName));
            }
            if (string.IsNullOrEmpty(routingKey))
            {
                throw new ArgumentException($"Argument {nameof(routingKey)} is null or empty.", nameof(routingKey));
            }

            var deadLetterExchanges = _exchanges.Select(x => x.Options.DeadLetterExchange).Distinct();
            if (!_exchanges.Any(x => x.Name == exchangeName) && !deadLetterExchanges.Any(x => x == exchangeName))
            {
                throw new ArgumentException($"Exchange {nameof(exchangeName)} has not been declared yet.", nameof(exchangeName));
            }
        }

        string GetDeadLetterExchange(string exchangeName)
        {
            var exchange = _exchanges.FirstOrDefault(x => x.Name == exchangeName);
            if (string.IsNullOrEmpty(exchange.Options.DeadLetterExchange))
            {
                throw new ArgumentException($"Exchange {nameof(exchangeName)} has not been configured with a dead letter exchange.", nameof(exchangeName));
            }

            return exchange.Options.DeadLetterExchange;
        }

        string DeclareDelayedQueue(string exchange, string deadLetterExchange, string routingKey, int secondsDelay)
        {
            var delayedQueueName = $"{routingKey}.delayed.{secondsDelay}";
            var arguments = CreateArguments(exchange, routingKey, secondsDelay);

            _channel.QueueDeclare(
                queue: delayedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments);

            _channel.QueueBind(
                queue: delayedQueueName,
                exchange: deadLetterExchange,
                routingKey: delayedQueueName);
            return delayedQueueName;
        }

        static Dictionary<string, object> CreateArguments(string exchangeName, string routingKey, int secondsDelay) =>
            new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", exchangeName },
                { "x-dead-letter-routing-key", routingKey },
                { "x-message-ttl", secondsDelay * 1000 },
                { "x-expires", secondsDelay * 1000 + QueueExpirationTime }
            };

        static IConnection CreateRabbitMqConnection(RabbitMqClientOptions options)
        {
            var factory = new ConnectionFactory
            {
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = options.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = options.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                RequestedHeartbeat = options.RequestedHeartbeat
            };

            if (options.TcpEndpoints?.Any() == true)
            {
                var clientEndpoints = options.TcpEndpoints.Select(x => new AmqpTcpEndpoint(x.HostName, x.Port)).ToList();
                return factory.CreateConnection(clientEndpoints);
            }

            return string.IsNullOrEmpty(options.ClientProvidedName)
                ? CreateConnection(options, factory)
                : CreateNamedConnection(options, factory);
        }

        static IConnection CreateNamedConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return factory.CreateConnection(options.HostNames.ToList(), options.ClientProvidedName);
            }

            factory.HostName = options.HostName;
            return factory.CreateConnection(options.ClientProvidedName);
        }

        static IConnection CreateConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return factory.CreateConnection(options.HostNames.ToList());
            }

            factory.HostName = options.HostName;
            return factory.CreateConnection();
        }
    }
}