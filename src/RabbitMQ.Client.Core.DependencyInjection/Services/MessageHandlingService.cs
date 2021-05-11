using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc/>
    public class MessageHandlingService : IMessageHandlingService
    {
        private readonly IEnumerable<MessageHandlerContainer> _messageHandlerContainers;
        private readonly ILoggingService _loggingService;

        public MessageHandlingService(
            IMessageHandlerContainerBuilder messageHandlerContainerBuilder,
            ILoggingService loggingService)
        {
            _messageHandlerContainers = messageHandlerContainerBuilder.BuildCollection();
            _loggingService = loggingService;
        }

        /// <inheritdoc />
        public async Task HandleMessageReceivingEvent(MessageHandlingContext context)
        {
            var eventArgs = context.Message;
            _loggingService.LogInformation($"A new message received with deliveryTag {eventArgs.DeliveryTag}.");
            var matchingRoutes = GetMatchingRoutePatterns(eventArgs.Exchange, eventArgs.RoutingKey);
            await ProcessMessageEvent(eventArgs, matchingRoutes).ConfigureAwait(false);
            if (context.AutoAckEnabled)
            {
                context.AckAction.Invoke(eventArgs);
            }
            _loggingService.LogInformation($"Message processing finished successfully. Acknowledge has been sent with deliveryTag {eventArgs.DeliveryTag}.");
        }

        private IEnumerable<string> GetMatchingRoutePatterns(string exchange, string routingKey)
        {
            var tree = _messageHandlerContainers.FirstOrDefault(x => x.Exchange == exchange)?.Tree ??
                _messageHandlerContainers.FirstOrDefault(x => x.IsGeneral)?.Tree;
            if (tree is null)
            {
                return Enumerable.Empty<string>();
            }

            var routingKeyParts = routingKey.Split(".");
            return WildcardExtensions.GetMatchingRoutePatterns(tree, routingKeyParts).ToList();
        }

        private async Task ProcessMessageEvent(BasicDeliverEventArgs eventArgs, IEnumerable<string> matchingRoutes)
        {
            var container = _messageHandlerContainers.FirstOrDefault(x => x.Exchange == eventArgs.Exchange) ??
                _messageHandlerContainers.FirstOrDefault(x => x.IsGeneral);
            if (container is null)
            {
                return;
            }

            var messageHandlerOrderingContainers = new List<MessageHandlerOrderingContainer>();
            foreach (var matchingRoute in matchingRoutes)
            {
                if (!container.MessageHandlers.ContainsKey(matchingRoute))
                {
                    continue;
                }

                var orderingContainers = container.MessageHandlers[matchingRoute]
                    .Select(handler => new MessageHandlerOrderingContainer(handler, matchingRoute, container.GetOrderForHandler(handler)));
                messageHandlerOrderingContainers.AddRange(orderingContainers);
            }

            var executedHandlers = new List<Type>();
            var orderedContainers = messageHandlerOrderingContainers.OrderByDescending(x => x.Order)
                .ThenByDescending(x => x.MessageHandler.GetHashCode())
                .ToList();
            foreach (var orderedContainer in orderedContainers)
            {
                var handlerType = orderedContainer.MessageHandler.GetType();
                if (executedHandlers.Contains(handlerType))
                {
                    continue;
                }

                switch (orderedContainer.MessageHandler)
                {
                    case IMessageHandler messageHandler:
                        RunMessageHandler(messageHandler, eventArgs, orderedContainer.MatchingRoute);
                        break;
                    case IAsyncMessageHandler asyncMessageHandler:
                        await RunAsyncMessageHandler(asyncMessageHandler, eventArgs, orderedContainer.MatchingRoute).ConfigureAwait(false);
                        break;
                    default:
                        throw new NotSupportedException($"The type {orderedContainer.MessageHandler.GetType()} of message handler is not supported.");
                }

                executedHandlers.Add(handlerType);
            }
        }

        private void RunMessageHandler(IMessageHandler handler, BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            ValidateMessageHandler(handler);
            _loggingService.LogDebug($"Starting processing the message by message handler {handler.GetType().Name}");
            handler.Handle(eventArgs, matchingRoute);
            _loggingService.LogDebug($"The message has been processed by message handler {handler.GetType().Name}");
        }

        private async Task RunAsyncMessageHandler(IAsyncMessageHandler handler, BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            ValidateMessageHandler(handler);
            _loggingService.LogDebug($"Starting processing the message by async message handler {handler.GetType().Name}");
            await handler.Handle(eventArgs, matchingRoute);
            _loggingService.LogDebug($"The message has been processed by async message handler {handler.GetType().Name}");
        }

        private static void ValidateMessageHandler<T>(T messageHandler)
        {
            if (messageHandler is null)
            {
                throw new ArgumentNullException(nameof(messageHandler), "Message handler is null.");
            }
        }
    }
}