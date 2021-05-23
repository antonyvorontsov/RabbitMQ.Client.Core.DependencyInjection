using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Middlewares;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace Examples.ManualAck.Middlewares
{
    public class CustomMessageHandlingMiddleware : IMessageHandlingMiddleware
    {
        public async Task Handle(MessageHandlingContext context, Func<Task> next)
        {
            // Execute the next action in the middleware pipeline.
            // Message handlers will be executed.
            await next();
            
            context.AcknowledgeMessage();
        }

        public async Task HandleError(MessageHandlingContext context, Exception exception, Func<Task> next)
        {
            await next();
            
            context.AcknowledgeMessage();
            throw exception;
        }
    }
}