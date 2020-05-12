using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
    {
        public Task Handle(string message, string routingKey, IQueueService queueService)
        {
            Console.WriteLine($"{message}:{routingKey}:{queueService.GetType()}");
            return Task.CompletedTask;
        }
    }
}