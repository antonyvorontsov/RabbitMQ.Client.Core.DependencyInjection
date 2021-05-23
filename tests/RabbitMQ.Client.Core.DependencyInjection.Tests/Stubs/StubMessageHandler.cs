using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubMessageHandler : IMessageHandler
    {
        private readonly IStubCaller _caller;

        public StubMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public void Handle(MessageHandlingContext context, string matchingRoute)
        {
            _caller.Call($"{context.Message.GetMessage()}:{matchingRoute}");
        }
    }
}