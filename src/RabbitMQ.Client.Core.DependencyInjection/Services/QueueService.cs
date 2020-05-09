using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <summary>
    /// Implementation of the custom RabbitMQ queue service.
    /// </summary>
    internal sealed class QueueService : IQueueService, IDisposable
    {
        public IConnection Connection { get; private set; }

        public IModel Channel { get; private set; }

        public IConnection ConsumingConnection { get; private set; }

        public IModel ConsumingChannel { get; private set; }

        AsyncEventingBasicConsumer _consumer;

        readonly IRabbitMqConnectionFactory _rabbitMqConnectionFactory;
        readonly IMessageHandlingService _messageHandlingService;
        readonly IEnumerable<RabbitMqExchange> _exchanges;
        readonly ILogger<QueueService> _logger;

        bool _consumingStarted;
        readonly object _lock = new object();

        const int QueueExpirationTime = 60000;

        public QueueService(
            Guid guid,
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<RabbitMqConnectionOptionsContainer> connectionOptionsContainers,
            IMessageHandlingService messageHandlingService,
            IEnumerable<RabbitMqExchange> exchanges,
            ILogger<QueueService> logger)
        {
            var optionsContainer = connectionOptionsContainers.FirstOrDefault(x => x.Guid == guid);
            if (optionsContainer is null)
            {
                throw new ArgumentException($"Connection options container for {nameof(QueueService)} with the guid {guid} is not found.", nameof(connectionOptionsContainers));
            }

            _rabbitMqConnectionFactory = rabbitMqConnectionFactory;
            _messageHandlingService = messageHandlingService;
            _exchanges = exchanges;
            _logger = logger;

            ConfigureConnectionInfrastructure(optionsContainer);
            StartClient();
        }

        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.CallbackException -= HandleConnectionCallbackException;
                if (Connection is IAutorecoveringConnection connection)
                {
                    connection.ConnectionRecoveryError -= HandleConnectionRecoveryError;
                }
            }

            if (ConsumingConnection != null)
            {
                ConsumingConnection.CallbackException -= HandleConnectionCallbackException;
                if (Connection is IAutorecoveringConnection connection)
                {
                    connection.ConnectionRecoveryError -= HandleConnectionRecoveryError;
                }
            }

            if (Channel != null)
            {
                Channel.CallbackException -= HandleChannelCallbackException;
                Channel.BasicRecoverOk -= HandleChannelBasicRecoverOk;
            }

            if (ConsumingChannel != null)
            {
                ConsumingChannel.CallbackException -= HandleChannelCallbackException;
                ConsumingChannel.BasicRecoverOk -= HandleChannelBasicRecoverOk;
            }

            if (Channel?.IsOpen == true)
            {
                Channel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (ConsumingChannel?.IsOpen == true)
            {
                ConsumingChannel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (Connection?.IsOpen == true)
            {
                Connection.Close();
            }

            if (ConsumingConnection?.IsOpen == true)
            {
                ConsumingConnection.Close();
            }

            Channel?.Dispose();
            Connection?.Dispose();
            ConsumingChannel?.Dispose();
            ConsumingConnection?.Dispose();
        }

        public void StartConsuming()
        {
            if (ConsumingChannel is null)
            {
                throw new ConsumingChannelIsNullException($"Consuming channel is null. Configure {nameof(IConsumingService)} or full functional {nameof(IQueueService)} for consuming messages.");
            }

            if (_consumingStarted)
            {
                return;
            }

            _consumer.Received += async (sender, eventArgs) => await _messageHandlingService.HandleMessageReceivingEvent(eventArgs, this);
            _consumingStarted = true;

            var consumptionExchanges = _exchanges.Where(x => x.IsConsuming);
            foreach (var exchange in consumptionExchanges)
            {
                foreach (var queue in exchange.Options.Queues)
                {
                    ConsumingChannel.BasicConsume(queue: queue.Name, autoAck: false, consumer: _consumer);
                }
            }
        }

        public void Send<T>(T @object, string exchangeName, string routingKey) where T : class
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var json = JsonConvert.SerializeObject(@object);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        public void Send<T>(T @object, string exchangeName, string routingKey, int secondsDelay) where T : class
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            Send(@object, deadLetterExchange, delayedQueueName);
        }

        public void SendJson(string json, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        public void SendJson(string json, string exchangeName, string routingKey, int secondsDelay)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            SendJson(json, deadLetterExchange, delayedQueueName);
        }

        public void SendString(string message, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(message);
            Send(bytes, CreateProperties(), exchangeName, routingKey);
        }

        public void SendString(string message, string exchangeName, string routingKey, int secondsDelay)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, secondsDelay);
            SendString(message, deadLetterExchange, delayedQueueName);
        }

        public void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            lock (_lock)
            {
                Channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        public void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay)
        {
            EnsureProducingChannelIsNotNull();
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

        public async Task SendAsync(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey, int secondsDelay) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey, secondsDelay)).ConfigureAwait(false);

        IBasicProperties CreateProperties()
        {
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }

        IBasicProperties CreateJsonProperties()
        {
            var properties = Channel.CreateBasicProperties();
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

        void ConfigureConnectionInfrastructure(RabbitMqConnectionOptionsContainer optionsContainer)
        {
            Connection = _rabbitMqConnectionFactory.CreateRabbitMqConnection(optionsContainer?.Options?.ProducerOptions);
            if (Connection != null)
            {
                Connection.CallbackException += HandleConnectionCallbackException;
                if (Connection is IAutorecoveringConnection connection)
                {
                    connection.ConnectionRecoveryError += HandleConnectionRecoveryError;
                }
                Channel = Connection.CreateModel();
                Channel.CallbackException += HandleChannelCallbackException;
                Channel.BasicRecoverOk += HandleChannelBasicRecoverOk;
            }

            ConsumingConnection = _rabbitMqConnectionFactory.CreateRabbitMqConnection(optionsContainer?.Options?.ConsumerOptions);
            if (ConsumingConnection != null)
            {
                ConsumingConnection.CallbackException += HandleConnectionCallbackException;
                if (Connection is IAutorecoveringConnection connection)
                {
                    connection.ConnectionRecoveryError += HandleConnectionRecoveryError;
                }
                ConsumingChannel = ConsumingConnection.CreateModel();
                ConsumingChannel.CallbackException += HandleChannelCallbackException;
                ConsumingChannel.BasicRecoverOk += HandleChannelBasicRecoverOk;
                
                _consumer = _rabbitMqConnectionFactory.CreateConsumer(ConsumingChannel);
            }
        }

        void StartClient()
        {
            var deadLetterExchanges = _exchanges
                .Where(x => !string.IsNullOrEmpty(x.Options.DeadLetterExchange))
                .Select(x => x.Options.DeadLetterExchange)
                .Distinct()
                .ToList();

            StartChannel(Channel, _exchanges, deadLetterExchanges);
            StartChannel(ConsumingChannel, _exchanges, deadLetterExchanges);
        }

        static void StartChannel(IModel channel, IEnumerable<RabbitMqExchange> exchanges, IEnumerable<string> deadLetterExchanges)
        {
            if (channel is null)
            {
                return;
            }

            foreach (var exchangeName in deadLetterExchanges)
            {
                StartDeadLetterExchange(channel, exchangeName);
            }

            foreach (var exchange in exchanges)
            {
                StartExchange(channel, exchange);
            }
        }

        static void StartDeadLetterExchange(IModel channel, string exchangeName)
        {
            channel.ExchangeDeclare(
                exchange: exchangeName,
                type: "direct",
                durable: true,
                autoDelete: false,
                arguments: null);
        }

        static void StartExchange(IModel channel, RabbitMqExchange exchange)
        {
            channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Options.Type,
                durable: exchange.Options.Durable,
                autoDelete: exchange.Options.AutoDelete,
                arguments: exchange.Options.Arguments);

            foreach (var queue in exchange.Options.Queues)
            {
                StartQueue(channel, queue, exchange.Name);
            }
        }

        static void StartQueue(IModel channel, RabbitMqQueueOptions queue, string exchangeName)
        {
            channel.QueueDeclare(queue: queue.Name,
                    durable: queue.Durable,
                    exclusive: queue.Exclusive,
                    autoDelete: queue.AutoDelete,
                    arguments: queue.Arguments);

            if (queue.RoutingKeys.Count > 0)
            {
                foreach (var route in queue.RoutingKeys)
                {
                    channel.QueueBind(
                        queue: queue.Name,
                        exchange: exchangeName,
                        routingKey: route);
                }
            }
            else
            {
                // If there are not any routing keys then make a bind with a queue name.
                channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchangeName,
                    routingKey: queue.Name);
            }
        }

        void EnsureProducingChannelIsNotNull()
        {
            if (Channel is null)
            {
                throw new ProducingChannelIsNullException($"Producing channel is null. Configure {nameof(IProducingService)} or full functional {nameof(IQueueService)} for producing messages.");
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
            if (string.IsNullOrEmpty(exchange?.Options?.DeadLetterExchange))
            {
                throw new ArgumentException($"Exchange {nameof(exchangeName)} has not been configured with a dead letter exchange.", nameof(exchangeName));
            }

            return exchange.Options.DeadLetterExchange;
        }

        string DeclareDelayedQueue(string exchange, string deadLetterExchange, string routingKey, int secondsDelay)
        {
            var delayedQueueName = $"{routingKey}.delayed.{secondsDelay}";
            var arguments = CreateArguments(exchange, routingKey, secondsDelay);

            Channel.QueueDeclare(
                queue: delayedQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments);

            Channel.QueueBind(
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
    }
}