using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Models
{
    public record HandleMessageReceivingEventTestDataModel
    {
        public string MessageRoutingKey { get; init; }

        public string MessageExchange { get; init; }

        public string MessageHandlerExchange { get; init; }

        public List<string> MessageHandlerPatterns { get; init; }

        public bool MessageHandlerShouldTrigger { get; init; }

        public int? MessageHandlerOrder { get; init; }

        public string AsyncMessageHandlerExchange { get; init; }

        public List<string> AsyncMessageHandlerPatterns { get; init; }

        public bool AsyncMessageHandlerShouldTrigger { get; init; }

        public int? AsyncMessageHandlerOrder { get; init; }
    }
}