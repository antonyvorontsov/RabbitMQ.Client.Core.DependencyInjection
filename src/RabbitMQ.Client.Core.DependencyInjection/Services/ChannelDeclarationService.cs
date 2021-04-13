using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc/>
    public class ChannelDeclarationService : IChannelDeclarationService
    {
        private readonly RabbitMqConnectionOptions _connectionOptions;
        private readonly IProducingService _producingService;
        private readonly IConsumingService _consumingService;
        private readonly IRabbitMqConnectionFactory _rabbitMqConnectionFactory;
        private readonly IEnumerable<RabbitMqExchange> _exchanges;
        private readonly ILoggingService _loggingService;
        
        public ChannelDeclarationService(
            IProducingService producingService,
            IConsumingService consumingService,
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IOptions<RabbitMqConnectionOptions> connectionOptions,
            IEnumerable<RabbitMqExchange> exchanges,
            ILoggingService loggingService)
        {
            _producingService = producingService;
            _consumingService = consumingService;
            _rabbitMqConnectionFactory = rabbitMqConnectionFactory;
            _connectionOptions = connectionOptions.Value;
            _exchanges = exchanges;
            _loggingService = loggingService;
        }

        /// <inheritdoc/>
        public void SetConnectionInfrastructureForRabbitMqServices()
        {
            if (_connectionOptions.ProducerOptions != null)
            {
                var connection = CreateConnection(_connectionOptions.ProducerOptions).EnsureIsNotNull();
                var channel = CreateChannel(connection);
                StartClient(channel);
                _producingService.UseConnection(connection);
                _producingService.UseChannel(channel);
            }

            if (_connectionOptions.ConsumerOptions != null)
            {
                var connection = CreateConnection(_connectionOptions.ConsumerOptions).EnsureIsNotNull();
                var channel = CreateChannel(connection);
                StartClient(channel);
                var consumer = _rabbitMqConnectionFactory.CreateConsumer(channel);
                _consumingService.UseConnection(connection);
                _consumingService.UseChannel(channel);
                _consumingService.UseConsumer(consumer);
            }
        }

        private IConnection? CreateConnection(RabbitMqServiceOptions options) => _rabbitMqConnectionFactory.CreateRabbitMqConnection(options);

        private IModel CreateChannel(IConnection connection)
        {
            connection.CallbackException += HandleConnectionCallbackException;
            if (connection is IAutorecoveringConnection recoveringConnection)
            {
                recoveringConnection.ConnectionRecoveryError += HandleConnectionRecoveryError;
            }
            
            var channel = connection.CreateModel();
            channel.CallbackException += HandleChannelCallbackException;
            channel.BasicRecoverOk += HandleChannelBasicRecoverOk;
            return channel;
        }

        private void StartClient(IModel channel)
        {
            var deadLetterExchanges = _exchanges
                .Select(x => x.Options)
                .Where(x => x.RequeueFailedMessages && !string.IsNullOrEmpty(x.DeadLetterExchange))
                .Select(x => new DeadLetterExchange(x.DeadLetterExchange, x.DeadLetterExchangeType))
                .Distinct(new DeadLetterExchangeEqualityComparer())
                .ToList();

            StartChannel(channel, _exchanges, deadLetterExchanges);
        }

        private static void StartChannel(IModel channel, IEnumerable<RabbitMqExchange> exchanges, IEnumerable<DeadLetterExchange> deadLetterExchanges)
        {
            foreach (var exchangeName in deadLetterExchanges)
            {
                StartDeadLetterExchange(channel, exchangeName);
            }

            foreach (var exchange in exchanges)
            {
                StartExchange(channel, exchange);
            }
        }

        private static void StartDeadLetterExchange(IModel channel, DeadLetterExchange exchange)
        {
            channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Type,
                durable: true,
                autoDelete: false,
                arguments: null);
        }

        private static void StartExchange(IModel channel, RabbitMqExchange exchange)
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

        private static void StartQueue(IModel channel, RabbitMqQueueOptions queue, string exchangeName)
        {
            channel.QueueDeclare(
                queue: queue.Name,
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

        private void HandleConnectionCallbackException(object sender, CallbackExceptionEventArgs? @event)
        {
            if (@event?.Exception is null)
            {
                return;
            }

            _loggingService.LogError(@event.Exception, @event.Exception.Message);
            throw @event.Exception;
        }

        private void HandleConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs? @event)
        {
            if (@event?.Exception is null)
            {
                return;
            }

            _loggingService.LogError(@event.Exception, @event.Exception.Message);
            throw @event.Exception;
        }

        private void HandleChannelBasicRecoverOk(object sender, EventArgs? @event)
        {
            if (@event is null)
            {
                return;
            }
            
            _loggingService.LogInformation("Connection has been reestablished");
        }

        private void HandleChannelCallbackException(object sender, CallbackExceptionEventArgs? @event)
        {
            if (@event?.Exception is null)
            {
                return;
            }

            _loggingService.LogError(@event.Exception, @event.Exception.Message);
        }
    }
}