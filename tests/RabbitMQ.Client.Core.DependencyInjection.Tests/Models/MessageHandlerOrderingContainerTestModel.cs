using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Models
{
    internal record MessageHandlerOrderingContainerTestModel
    {
        public IBaseMessageHandler MessageHandler { get; init; }

        public bool ShouldTrigger { get; init; }

        public int? OrderValue { get; init; }

        public int? CallOrder { get; set; }
    }
}