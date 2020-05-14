using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubNonCyclicMessageHandler : INonCyclicMessageHandler
    {
        readonly IStubCaller _caller;
        
        public StubNonCyclicMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }
        
        public void Handle(string message, string routingKey, IQueueService queueService)
        {
            _caller.Call($"{message}:{routingKey}:{queueService.GetType()}");
        }
    }
}