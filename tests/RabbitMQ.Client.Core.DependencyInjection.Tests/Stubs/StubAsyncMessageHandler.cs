using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubAsyncMessageHandler : IAsyncMessageHandler
    {
        readonly IStubCaller _caller;

        public StubAsyncMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public async Task Handle(string message, string routingKey)
        {
            await _caller.CallAsync($"{message}:{routingKey}");
        }
    }
}