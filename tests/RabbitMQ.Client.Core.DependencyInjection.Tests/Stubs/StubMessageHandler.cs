using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubMessageHandler : IMessageHandler
    {
        readonly IStubCaller _caller;

        public StubMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            _caller.Call($"{eventArgs.GetMessage()}:{matchingRoute}");
        }
    }
}