using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class HandleMessageReceivingEventTestDataModel
    {
        public string MessageRoutingKey { get; set; }

        public List<string> MessageHandlerPatterns { get; set; }
            
        public bool MessageHandlerShouldTrigger { get; set; }
            
        public List<string> AsyncMessageHandlerPatterns { get; set; }
            
        public bool AsyncMessageHandlerShouldTrigger { get; set; }            
            
        public List<string> NonCyclicMessageHandlerPatterns { get; set; }
            
        public bool NonCyclicMessageHandlerShouldTrigger { get; set; }
            
        public List<string> AsyncNonCyclicMessageHandlerPatterns { get; set; }
            
        public bool AsyncNonCyclicMessageHandlerShouldTrigger { get; set; }
    }
}