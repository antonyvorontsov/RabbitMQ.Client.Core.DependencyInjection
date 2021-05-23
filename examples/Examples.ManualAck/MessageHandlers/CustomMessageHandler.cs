using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace Examples.ManualAck.MessageHandlers
{
    public class CustomMessageHandler : IMessageHandler
    {
        private readonly IProducingService _producingService;
        
        public CustomMessageHandler(IProducingService producingService)
        {
            _producingService = producingService;
        }
        
        public void Handle(MessageHandlingContext context, string matchingRoute)
        {
            // Do anything you want.
            // E.g. send a message to the exchange with another routing key. Whatever.
            _producingService.Send(context.Message.Body, context.Message.BasicProperties, "exchange", "other.routing.key");
        }
    }
}