using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc />
    public class ErrorProcessingService : IErrorProcessingService
    {
        private readonly IProducingService _producingService;
        private readonly IEnumerable<RabbitMqExchange> _exchanges;
        private readonly ILoggingService _loggingService;

        public ErrorProcessingService(
            IProducingService producingService,
            IEnumerable<RabbitMqExchange> exchanges,
            ILoggingService loggingService)
        {
            _producingService = producingService;
            _exchanges = exchanges;
            _loggingService = loggingService;
        }

        /// <inheritdoc />
        public virtual async Task HandleMessageProcessingFailure(MessageHandlingContext context, Exception exception)
        {
            var eventArgs = context.Message;
            if (context.AutoAckEnabled)
            {
                context.AcknowledgeMessage();
            }

            _loggingService.LogError(exception, $"An error occurred while processing received message with the delivery tag {eventArgs.DeliveryTag}.");
            await HandleFailedMessageProcessing(eventArgs).ConfigureAwait(false);
        }

        protected async Task HandleFailedMessageProcessing(BasicDeliverEventArgs eventArgs)
        {
            var exchange = _exchanges.FirstOrDefault(x => x.Name == eventArgs.Exchange);
            if (exchange is null)
            {
                _loggingService.LogWarning($"Could not detect an exchange \"{eventArgs.Exchange}\" to determine the necessity of resending the failed message. The message won't be re-queued");
                return;
            }
            
            if (!exchange.Options.RequeueFailedMessages)
            {
                _loggingService.LogWarning($"RequeueFailedMessages option for an exchange \"{eventArgs.Exchange}\" is disabled. The message won't be re-queued");
                return;
            }

            if (string.IsNullOrEmpty(exchange.Options.DeadLetterExchange))
            {
                _loggingService.LogWarning($"DeadLetterExchange has not been configured for an exchange \"{eventArgs.Exchange}\". The message won't be re-queued");
                return;
            }

            if (exchange.Options.RequeueTimeoutMilliseconds < 1)
            {
                _loggingService.LogWarning($"The value RequeueTimeoutMilliseconds for an exchange \"{eventArgs.Exchange}\" less than 1 millisecond. Configuration is invalid. The message won't be re-queued");
                return;
            }
            
            if (exchange.Options.RequeueAttempts < 1)
            {
                _loggingService.LogWarning($"The value RequeueAttempts for an exchange \"{eventArgs.Exchange}\" less than 1. Configuration is invalid. The message won't be re-queued");
                return;
            }

            if (eventArgs.BasicProperties.Headers is null)
            {
                eventArgs.BasicProperties.Headers = new Dictionary<string, object>();
            }

            if (!eventArgs.BasicProperties.Headers.ContainsKey("re-queue-attempts"))
            {
                eventArgs.BasicProperties.Headers.Add("re-queue-attempts", 1);
                await RequeueMessage(eventArgs, exchange.Options.RequeueTimeoutMilliseconds);
                return;
            }
            
            var currentAttempt = (int)eventArgs.BasicProperties.Headers["re-queue-attempts"];
            if (currentAttempt < exchange.Options.RequeueAttempts)
            {
                eventArgs.BasicProperties.Headers["re-queue-attempts"] = currentAttempt + 1;
                await RequeueMessage(eventArgs, exchange.Options.RequeueTimeoutMilliseconds);
            }
            else
            {
                _loggingService.LogInformation("The failed message would not be re-queued. Attempts limit exceeded");   
            }
        }
        
        protected async Task RequeueMessage(BasicDeliverEventArgs eventArgs, int timeoutMilliseconds)
        {
            await _producingService.SendAsync(eventArgs.Body, eventArgs.BasicProperties, eventArgs.Exchange, eventArgs.RoutingKey, timeoutMilliseconds);
            _loggingService.LogInformation("The failed message has been re-queued");
        }
    }
}