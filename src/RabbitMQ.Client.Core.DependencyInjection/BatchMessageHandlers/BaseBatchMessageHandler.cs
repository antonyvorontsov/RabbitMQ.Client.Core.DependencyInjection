using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers
{
    public abstract class BaseBatchMessageHandler : IHostedService, IDisposable
    {
        protected virtual TimeSpan DueTo { get; set; } = TimeSpan.Zero;
        
        protected abstract TimeSpan Period { get; set; }

        protected virtual uint PrefetchSize { get; set; } = 0;
        
        protected abstract string QueueName { get; set; }

        protected abstract ushort PrefetchCount { get; set; }

        readonly RabbitMqClientOptions _clientOptions;
        readonly ILogger<BaseBatchMessageHandler> _logger;
        Timer _timer;

        protected BaseBatchMessageHandler(
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            ILogger<BaseBatchMessageHandler> logger)
        {
            var optionsContainer = batchConsumerConnectionOptions.FirstOrDefault(x => x.Type == GetType());
            if (optionsContainer is null)
            {
                throw new ArgumentNullException($"Client connection options for {nameof(BaseBatchMessageHandler)} has not been found.", nameof(batchConsumerConnectionOptions));
            }

            _clientOptions = optionsContainer.ClientOptions ?? throw new ArgumentNullException($"Consumer client options is null for {nameof(BaseBatchMessageHandler)}.", nameof(optionsContainer.ClientOptions));
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ValidateProperties();
            _logger.LogInformation("BatchMessageHandler has been started.");
            _timer = new Timer(
                async state => await StartPeriodicJob(cancellationToken),
                null,
                DueTo,
                Period);
            return Task.CompletedTask;
        }

        void ValidateProperties()
        {
            if (string.IsNullOrEmpty(QueueName))
            {
                throw new BatchMessageHandlerInvalidPropertyValueException("Queue name could not be empty.", nameof(QueueName));
            }

            if (PrefetchCount < 1)
            {
                throw new BatchMessageHandlerInvalidPropertyValueException("PrefetchCount value should be more than one.", nameof(PrefetchCount));
            }
        }

        async Task StartPeriodicJob(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using var connection = RabbitMqFactoryExtensions.CreateRabbitMqConnection(_clientOptions);
                using var channel = connection.CreateModel();
            
                channel.BasicQos(PrefetchSize, PrefetchCount, false);
            
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (sender, eventArgs) => await HandleMessageReceivingEvent(eventArgs);
                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            }
        }
        
        async Task HandleMessageReceivingEvent(BasicDeliverEventArgs eventArgs)
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            _logger.LogInformation($"A new message was received with deliveryTag {eventArgs.DeliveryTag}.");
            _logger.LogInformation(message);
        }

        protected abstract Task HandleMessages();

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BatchMessageHandler has been stopped.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}