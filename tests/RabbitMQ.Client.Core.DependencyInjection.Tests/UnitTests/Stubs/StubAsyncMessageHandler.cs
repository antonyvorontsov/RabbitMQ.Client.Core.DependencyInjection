using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests.Stubs
{
    internal class StubAsyncMessageHandler : IAsyncMessageHandler
    {
        public Task Handle(string message, string routingKey)
        {
            Console.WriteLine($"{message}:{routingKey}");
            return Task.CompletedTask;
        }
    }
}