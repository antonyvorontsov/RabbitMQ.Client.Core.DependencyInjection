using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc cref="IProducingService"/>
    public sealed class ProducingService : IProducingService, IDisposable
    {
        /// <inheritdoc/>
        public IConnection? Connection { get; private set; }

        /// <inheritdoc/>
        public IModel? Channel { get; private set; }

        private readonly IEnumerable<RabbitMqExchange> _exchanges;
        private readonly object _lock = new object();

        private const int QueueExpirationTime = 60000;

        public ProducingService(IEnumerable<RabbitMqExchange> exchanges)
        {
            _exchanges = exchanges;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Channel?.IsOpen == true)
            {
                Channel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (Connection?.IsOpen == true)
            {
                Connection.Close();
            }

            Channel?.Dispose();
            Connection?.Dispose();
        }

        /// <inheritdoc/>
        public void UseConnection(IConnection connection)
        {
            Connection = connection;
        }

        /// <inheritdoc/>
        public void UseChannel(IModel channel)
        {
            Channel = channel;
        }

        /// <inheritdoc/>
        public void Send<T>(T @object, string exchangeName, string routingKey) where T : class
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var json = JsonConvert.SerializeObject(@object);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public void Send<T>(T @object, string exchangeName, string routingKey, int millisecondsDelay) where T : class
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, millisecondsDelay);
            Send(@object, deadLetterExchange, delayedQueueName);
        }

        /// <inheritdoc/>
        public void SendJson(string json, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(json);
            var properties = CreateJsonProperties();
            Send(bytes, properties, exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public void SendJson(string json, string exchangeName, string routingKey, int millisecondsDelay)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, millisecondsDelay);
            SendJson(json, deadLetterExchange, delayedQueueName);
        }

        /// <inheritdoc/>
        public void SendString(string message, string exchangeName, string routingKey)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var bytes = Encoding.UTF8.GetBytes(message);
            Send(bytes, CreateProperties(), exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public void SendString(string message, string exchangeName, string routingKey, int millisecondsDelay)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, millisecondsDelay);
            SendString(message, deadLetterExchange, delayedQueueName);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey, int millisecondsDelay)
        {
            EnsureProducingChannelIsNotNull();
            ValidateArguments(exchangeName, routingKey);
            var deadLetterExchange = GetDeadLetterExchange(exchangeName);
            var delayedQueueName = DeclareDelayedQueue(exchangeName, deadLetterExchange, routingKey, millisecondsDelay);
            Send(bytes, properties, deadLetterExchange, delayedQueueName);
        }

        /// <inheritdoc/>
        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendAsync<T>(T @object, string exchangeName, string routingKey, int millisecondsDelay) where T : class =>
            await Task.Run(() => Send(@object, exchangeName, routingKey, millisecondsDelay)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendJsonAsync(string json, string exchangeName, string routingKey) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendJsonAsync(string json, string exchangeName, string routingKey, int millisecondsDelay) =>
            await Task.Run(() => SendJson(json, exchangeName, routingKey, millisecondsDelay)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendStringAsync(string message, string exchangeName, string routingKey) =>
            await Task.Run(() => SendString(message, exchangeName, routingKey)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendStringAsync(string message, string exchangeName, string routingKey, int millisecondsDelay) =>
            await Task.Run(() => SendString(message, exchangeName, routingKey, millisecondsDelay)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendAsync(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey)).ConfigureAwait(false);

        /// <inheritdoc/>
        public async Task SendAsync(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey, int millisecondsDelay) =>
            await Task.Run(() => Send(bytes, properties, exchangeName, routingKey, millisecondsDelay)).ConfigureAwait(false);

        private IBasicProperties CreateProperties()
        {
            var properties = Channel.EnsureIsNotNull().CreateBasicProperties();
            properties.Persistent = true;
            return properties;
        }

        private IBasicProperties CreateJsonProperties()
        {
            var properties = Channel.EnsureIsNotNull().CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            return properties;
        }

        private void EnsureProducingChannelIsNotNull()
        {
            if (Channel is null)
            {
                throw new ProducingChannelIsNullException($"Producing channel is null. Configure {nameof(IProducingService)} or full functional {nameof(IProducingService)} for producing messages");
            }
        }

        private void ValidateArguments(string exchangeName, string routingKey)
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

        private string GetDeadLetterExchange(string exchangeName)
        {
            var exchange = _exchanges.FirstOrDefault(x => x.Name == exchangeName);
            if (string.IsNullOrEmpty(exchange?.Options?.DeadLetterExchange))
            {
                throw new ArgumentException($"Exchange {nameof(exchangeName)} has not been configured with a dead letter exchange.", nameof(exchangeName));
            }

            return exchange.Options.DeadLetterExchange;
        }

        private string DeclareDelayedQueue(string exchange, string deadLetterExchange, string routingKey, int millisecondsDelay)
        {
            var delayedQueueName = $"{routingKey}.delayed.{millisecondsDelay}";
            var arguments = CreateArguments(exchange, routingKey, millisecondsDelay);

            Channel.EnsureIsNotNull();
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

        private static Dictionary<string, object> CreateArguments(string exchangeName, string routingKey, int millisecondsDelay) =>
            new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", exchangeName },
                { "x-dead-letter-routing-key", routingKey },
                { "x-message-ttl", millisecondsDelay },
                { "x-expires", millisecondsDelay + QueueExpirationTime }
            };
    }
}