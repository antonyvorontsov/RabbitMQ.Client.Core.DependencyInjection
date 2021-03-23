using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc cref="IConsumingService"/>
    public class ConsumingService : IConsumingService, IDisposable
    {
        /// <inheritdoc/>
        public IConnection? Connection { get; private set; }

        /// <inheritdoc/>
        public IModel? Channel { get; private set; }
        
        /// <inheritdoc/>
        public AsyncEventingBasicConsumer? Consumer { get; private set; }

        private bool _consumingStarted;

        private readonly IMessageHandlingPipelineExecutingService _messageHandlingPipelineExecutingService;
        private readonly IEnumerable<RabbitMqExchange> _exchanges;

        private IEnumerable<string> _consumerTags = new List<string>();

        public ConsumingService(
            IMessageHandlingPipelineExecutingService messageHandlingPipelineExecutingService,
            IEnumerable<RabbitMqExchange> exchanges)
        {
            _messageHandlingPipelineExecutingService = messageHandlingPipelineExecutingService;
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
        public void UseConsumer(AsyncEventingBasicConsumer consumer)
        {
            Consumer = consumer;
        }

        /// <inheritdoc/>
        public void StartConsuming()
        {
            Channel.EnsureIsNotNull();
            Consumer.EnsureIsNotNull();

            if (_consumingStarted)
            {
                return;
            }
            
            Consumer.Received += ConsumerOnReceived;
            _consumingStarted = true;

            var consumptionExchanges = _exchanges.Where(x => x.IsConsuming);
            _consumerTags = consumptionExchanges.SelectMany(
                    exchange => exchange.Options.Queues.Select(
                        queue => Channel.BasicConsume(queue: queue.Name, autoAck: false, consumer: Consumer)))
                .Distinct()
                .ToList();
        }

        /// <inheritdoc/>
        public void StopConsuming()
        {
            Channel.EnsureIsNotNull();
            Consumer.EnsureIsNotNull();

            if (!_consumingStarted)
            {
                return;
            }

            Consumer.Received -= ConsumerOnReceived;
            _consumingStarted = false;
            foreach (var tag in _consumerTags)
            {
                Channel.BasicCancel(tag);
            }
        }

        private Task ConsumerOnReceived(object sender, BasicDeliverEventArgs eventArgs) => _messageHandlingPipelineExecutingService.Execute(eventArgs, this);
    }
}