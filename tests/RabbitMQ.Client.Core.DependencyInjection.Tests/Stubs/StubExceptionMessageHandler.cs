using System;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubExceptionMessageHandler : IMessageHandler
    {
        readonly IStubCaller _caller;

        public StubExceptionMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public void Handle(string message, string routingKey)
        {
            _caller.Call($"{message}:{routingKey}");
            throw new Exception();
        }
    }
}