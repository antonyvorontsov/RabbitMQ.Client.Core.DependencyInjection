using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class HandleMessageReceivingEventTestDataModel
    {
        public string MessageRoutingKey { get; set; }

        public string MessageExchange { get; set; }

        public string MessageHandlerExchange { get; set; }

        public List<string> MessageHandlerPatterns { get; set; }

        public bool MessageHandlerShouldTrigger { get; set; }

        public string AsyncMessageHandlerExchange { get; set; }

        public List<string> AsyncMessageHandlerPatterns { get; set; }

        public bool AsyncMessageHandlerShouldTrigger { get; set; }

        public string NonCyclicMessageHandlerExchange { get; set; }

        public List<string> NonCyclicMessageHandlerPatterns { get; set; }

        public bool NonCyclicMessageHandlerShouldTrigger { get; set; }

        public string AsyncNonCyclicMessageHandlerExchange { get; set; }

        public List<string> AsyncNonCyclicMessageHandlerPatterns { get; set; }

        public bool AsyncNonCyclicMessageHandlerShouldTrigger { get; set; }
    }
}