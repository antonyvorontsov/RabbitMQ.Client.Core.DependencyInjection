using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
    {
        readonly IStubCaller _caller;
        
        public StubAsyncNonCyclicMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }
        
        public async Task Handle(string message, string routingKey, IQueueService queueService)
        {
            await _caller.CallAsync($"{message}:{routingKey}:{queueService.GetType()}");
        }
    }
}