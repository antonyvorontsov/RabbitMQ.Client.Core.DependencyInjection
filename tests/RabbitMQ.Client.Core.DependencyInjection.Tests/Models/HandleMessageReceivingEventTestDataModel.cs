using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Models
{
    public class HandleMessageReceivingEventTestDataModel
    {
        public string MessageRoutingKey { get; set; }

        public string MessageExchange { get; set; }

        public string MessageHandlerExchange { get; set; }

        public List<string> MessageHandlerPatterns { get; set; }

        public bool MessageHandlerShouldTrigger { get; set; }

        public int? MessageHandlerOrder { get; set; }

        public string AsyncMessageHandlerExchange { get; set; }

        public List<string> AsyncMessageHandlerPatterns { get; set; }

        public bool AsyncMessageHandlerShouldTrigger { get; set; }

        public int? AsyncMessageHandlerOrder { get; set; }
    }
}