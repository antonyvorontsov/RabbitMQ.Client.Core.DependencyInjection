using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <summary>
    /// A message handler that handles messages in batches.
    /// </summary>
    public abstract class BaseBatchMessageHandler : IHostedService, IDisposable
    {
        /// <summary>
        /// A connection which is in use by batch message handler.
        /// </summary>
        public IConnection? Connection { get; private set; }

        /// <summary>
        /// A channel that has been created using the connection.
        /// </summary>
        public IModel? Channel { get;  private set; }

        /// <summary>
        /// Prefetch size value that can be overridden.
        /// </summary>
        public virtual uint PrefetchSize { get; set; } = 0;

        /// <summary>
        /// Queue name which will be read by that batch message handler.
        /// </summary>
        public abstract string QueueName { get; set; }

        /// <summary>
        /// Prefetch count value (batch size).
        /// </summary>
        public abstract ushort PrefetchCount { get; set; }
        
        /// <summary>
        /// The TimeSpan period through which messages will be processing.
        /// </summary>
        public virtual TimeSpan? MessageHandlingPeriod { get; set; }

        private readonly IRabbitMqConnectionFactory _rabbitMqConnectionFactory;
        private readonly RabbitMqServiceOptions _serviceOptions;
        private readonly IEnumerable<IBatchMessageHandlingFilter> _batchMessageHandlingFilters;
        private readonly ILoggingService _loggingService;

        private readonly ConcurrentBag<BasicDeliverEventArgs> _messages = new ConcurrentBag<BasicDeliverEventArgs>();
        private Timer? _timer;
        private readonly object _lock = new object();
        private bool _disposed = false;

        protected BaseBatchMessageHandler(
            IRabbitMqConnectionFactory rabbitMqConnectionFactory,
            IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
            IEnumerable<IBatchMessageHandlingFilter> batchMessageHandlingFilters,
            ILoggingService loggingService)
        {
            var optionsContainer = batchConsumerConnectionOptions.FirstOrDefault(x => x.Type == GetType());
            if (optionsContainer is null)
            {
                throw new ArgumentNullException($"Client connection options for {nameof(BaseBatchMessageHandler)} has not been found.", nameof(batchConsumerConnectionOptions));
            }

            _serviceOptions = optionsContainer.ServiceOptions;
            _rabbitMqConnectionFactory = rabbitMqConnectionFactory;
            _batchMessageHandlingFilters = batchMessageHandlingFilters;
            _loggingService = loggingService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ValidateProperties();
            _loggingService.LogInformation($"Batch message handler {GetType()} has been started.");
            Connection = _rabbitMqConnectionFactory.CreateRabbitMqConnection(_serviceOptions).EnsureIsNotNull();
            Channel = Connection.CreateModel().EnsureIsNotNull();
            Channel.BasicQos(PrefetchSize, PrefetchCount, false);
            
            if (MessageHandlingPeriod != null)
            {
                _timer = new Timer(async _ => await ProcessBatchOfMessages(cancellationToken).ConfigureAwait(false), null, MessageHandlingPeriod.Value, MessageHandlingPeriod.Value);
            }

            var consumer = _rabbitMqConnectionFactory.CreateConsumer(Channel);
            consumer.Received += async (_, eventArgs) =>
            {
                lock (_lock)
                {
                    _messages.Add(eventArgs);
                    if (_messages.Count < PrefetchCount)
                    {
                        return;
                    }
                }

                await ProcessBatchOfMessages(cancellationToken).ConfigureAwait(false);
            };
            
            Channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessBatchOfMessages(CancellationToken cancellationToken)
        {
            var messages = GetMessages();
            if (!messages.Any())
            {
                return;
            }

            await ExecutePipeline(messages, cancellationToken).ConfigureAwait(false);
        }

        private async Task ExecutePipeline(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken)
        {
            Func<IEnumerable<BasicDeliverEventArgs>, CancellationToken, Task> handle = Handle;
            foreach (var filter in _batchMessageHandlingFilters.Reverse())
            {
                handle = filter.Execute(handle);
            }

            await handle(messages, cancellationToken).ConfigureAwait(false);
        }

        private async Task Handle(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken)
        {
            var messagesCollection = messages.ToList();
            await HandleMessages(messagesCollection, cancellationToken).ConfigureAwait(false);
            var latestDeliveryTag = messagesCollection.Max(x => x.DeliveryTag);
            Channel.EnsureIsNotNull().BasicAck(latestDeliveryTag, true);
        }

        private IList<BasicDeliverEventArgs> GetMessages()
        {
            lock (_lock)
            {
                if (!_messages.Any())
                {
                    return new List<BasicDeliverEventArgs>();
                }
                
                var messages = _messages.ToList();
                _messages.Clear();
                return messages;
            }
        }

        private void ValidateProperties()
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

        /// <summary>
        /// Handle a batch of messages.
        /// </summary>
        /// <param name="messages">A collection of messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public abstract Task HandleMessages(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken);
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _loggingService.LogInformation($"Batch message handler {GetType()} has been stopped.");
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer?.Dispose();
                Connection?.Dispose();
                Channel?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaseBatchMessageHandler()
        {
            Dispose(false);
        }
    }
}