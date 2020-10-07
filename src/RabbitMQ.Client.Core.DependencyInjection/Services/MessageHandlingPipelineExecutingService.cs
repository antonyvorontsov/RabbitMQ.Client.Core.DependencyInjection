using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <summary>
    /// Service that is responsible for handling received message in the pipeline of filters.
    /// </summary>
    public class MessageHandlingPipelineExecutingService : IMessageHandlingPipelineExecutingService
    {
        readonly IMessageHandlingService _messageHandlingService;
        readonly IEnumerable<IMessageHandlingFilter> _handlingFilters;
        readonly IEnumerable<IMessageHandlingExceptionFilter> _exceptionFilters;

        public MessageHandlingPipelineExecutingService(
            IMessageHandlingService messageHandlingService,
            IEnumerable<IMessageHandlingFilter> handlingFilters,
            IEnumerable<IMessageHandlingExceptionFilter> exceptionFilters)
        {
            _messageHandlingService = messageHandlingService;
            _handlingFilters = handlingFilters ?? Enumerable.Empty<IMessageHandlingFilter>();
            _exceptionFilters = exceptionFilters ?? Enumerable.Empty<IMessageHandlingExceptionFilter>();
        }

        /// <summary>
        /// Execute message handling pipeline.
        /// </summary>
        /// <param name="eventArgs">Received message.</param>
        /// <param name="queueService">Queuing service.</param>
        public async Task Execute(BasicDeliverEventArgs eventArgs, IQueueService queueService)
        {
            try
            {
                await ExecutePipeline(eventArgs, queueService).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExecuteFailurePipeline(exception, eventArgs, queueService).ConfigureAwait(false);
            }
        }

        async Task ExecutePipeline(BasicDeliverEventArgs eventArgs, IQueueService queueService)
        {
            Func<BasicDeliverEventArgs, IQueueService, Task> handle = _messageHandlingService.HandleMessageReceivingEvent;
            foreach (var filter in _handlingFilters.Reverse())
            {
                handle = filter.Execute(handle);
            }

            await handle(eventArgs, queueService).ConfigureAwait(false);
        }

        async Task ExecuteFailurePipeline(Exception exception, BasicDeliverEventArgs eventArgs, IQueueService queueService)
        {
            Func<Exception, BasicDeliverEventArgs, IQueueService, Task> handle = _messageHandlingService.HandleMessageProcessingFailure;
            foreach (var filter in _exceptionFilters.Reverse())
            {
                handle = filter.Execute(handle);
            }

            await handle(exception, eventArgs, queueService).ConfigureAwait(false);
        }
    }
}