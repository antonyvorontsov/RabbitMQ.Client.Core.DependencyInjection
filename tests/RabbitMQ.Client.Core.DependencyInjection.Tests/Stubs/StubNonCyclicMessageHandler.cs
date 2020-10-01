using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubNonCyclicMessageHandler : INonCyclicMessageHandler
    {
        readonly IStubCaller _caller;

        public StubNonCyclicMessageHandler(IStubCaller caller)
        {
            _caller = caller;
        }

        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute, IQueueService queueService)
        {
            _caller.Call($"{eventArgs.GetMessage()}:{matchingRoute}:{queueService.GetType()}");
        }
    }
}