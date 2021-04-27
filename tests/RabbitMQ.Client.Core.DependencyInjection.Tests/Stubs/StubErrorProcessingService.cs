using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubErrorProcessingService : IErrorProcessingService
    {
        public Task HandleMessageProcessingFailure(MessageHandlingContext context, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}