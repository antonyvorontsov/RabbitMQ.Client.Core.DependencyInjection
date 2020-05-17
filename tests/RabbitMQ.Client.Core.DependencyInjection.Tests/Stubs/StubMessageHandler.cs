using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubMessageHandler : IMessageHandler
    {
        readonly IStubCaller _caller;

        public StubMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public void Handle(string message, string routingKey)
        {
            _caller.Call($"{message}:{routingKey}");
        }
    }
}