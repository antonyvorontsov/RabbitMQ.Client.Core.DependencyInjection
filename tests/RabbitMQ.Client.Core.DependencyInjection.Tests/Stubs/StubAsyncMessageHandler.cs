using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubAsyncMessageHandler : IAsyncMessageHandler
    {
        private readonly IStubCaller _caller;

        public StubAsyncMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public async Task Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            await _caller.CallAsync($"{eventArgs.GetMessage()}:{matchingRoute}");
        }
    }
}