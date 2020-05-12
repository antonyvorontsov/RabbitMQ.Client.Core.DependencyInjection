using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubAsyncMessageHandler : IAsyncMessageHandler
    {
        public Task Handle(string message, string routingKey)
        {
            Console.WriteLine($"{message}:{routingKey}");
            return Task.CompletedTask;
        }
    }
}