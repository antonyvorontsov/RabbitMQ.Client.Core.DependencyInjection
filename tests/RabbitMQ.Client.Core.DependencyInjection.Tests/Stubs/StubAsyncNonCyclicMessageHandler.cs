using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
    {
        readonly IStubCaller _caller;

        public StubAsyncNonCyclicMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public async Task Handle(BasicDeliverEventArgs eventArgs, string matchingRoute, IQueueService queueService)
        {
            await _caller.CallAsync($"{eventArgs.GetMessage()}:{matchingRoute}:{queueService.GetType()}");
        }
    }
}