using System;

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