using System;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubExceptionMessageHandler : IMessageHandler
    {
        readonly IStubCaller _caller;

        public StubExceptionMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            _caller.Call($"{eventArgs.GetMessage()}:{matchingRoute}");
            throw new Exception();
        }
    }
}