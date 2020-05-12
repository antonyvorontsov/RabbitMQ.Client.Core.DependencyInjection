using System;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests.Stubs
{
    internal class StubMessageHandler : IMessageHandler
    {
        public void Handle(string message, string routingKey)
        {
            Console.WriteLine($"{message}:{routingKey}");
        }
    }
}