using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Middlewares;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc/>
    public class MessageHandlingPipelineExecutingService : IMessageHandlingPipelineExecutingService
    {
        private readonly IMessageHandlingService _messageHandlingService;
        private readonly IErrorProcessingService _errorProcessingService;
        private readonly IEnumerable<IMessageHandlingMiddleware> _messageHandlingMiddlewares;

        public MessageHandlingPipelineExecutingService(
            IMessageHandlingService messageHandlingService,
            IErrorProcessingService errorProcessingService,
            IEnumerable<IMessageHandlingMiddleware> messageHandlingMiddlewares)
        {
            _messageHandlingService = messageHandlingService;
            _errorProcessingService = errorProcessingService;
            _messageHandlingMiddlewares = messageHandlingMiddlewares;
        }

        /// <inheritdoc/>
        public async Task Execute(BasicDeliverEventArgs eventArgs, Action<BasicDeliverEventArgs> ackAction)
        {
            var context = new MessageHandlingContext(eventArgs, ackAction);
            try
            {
                await ExecutePipeline(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExecuteFailurePipeline(context, exception).ConfigureAwait(false);
            }
        }

        private async Task ExecutePipeline(MessageHandlingContext context)
        {
            if (!_messageHandlingMiddlewares.Any())
            {
                await _messageHandlingService.HandleMessageReceivingEvent(context);
                return;
            }

            Func<Task> handleFunction = async () => await _messageHandlingService.HandleMessageReceivingEvent(context);
            foreach (var middleware in _messageHandlingMiddlewares)
            {
                var previousHandleFunction = handleFunction;
                handleFunction = async () => await middleware.Handle(context, previousHandleFunction);
            }

            await handleFunction().ConfigureAwait(false);
        }

        private async Task ExecuteFailurePipeline(MessageHandlingContext context, Exception exception)
        {
            if (!_messageHandlingMiddlewares.Any())
            {
                await _errorProcessingService.HandleMessageProcessingFailure(context, exception);
                return;
            }

            Func<Task> handleFunction = async () => await _errorProcessingService.HandleMessageProcessingFailure(context, exception);
            foreach (var middleware in _messageHandlingMiddlewares)
            {
                var previousHandleFunction = handleFunction;
                handleFunction = async () => await middleware.HandleError(context, exception, previousHandleFunction);
            }

            await handleFunction().ConfigureAwait(false);
        }
    }
}