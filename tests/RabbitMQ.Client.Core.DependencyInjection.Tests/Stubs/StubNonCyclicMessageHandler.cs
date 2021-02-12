using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubNonCyclicMessageHandler : INonCyclicMessageHandler
    {
        private readonly IStubCaller _caller;

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