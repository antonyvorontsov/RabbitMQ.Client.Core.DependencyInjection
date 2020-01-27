using System;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests.Stubs
{
    internal class StubAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
    {
        public Task Handle(string message, string routingKey, IQueueService queueService)
        {
            Console.WriteLine($"{message}:{routingKey}:{queueService.GetType()}");
            return Task.CompletedTask;
        }
    }
}