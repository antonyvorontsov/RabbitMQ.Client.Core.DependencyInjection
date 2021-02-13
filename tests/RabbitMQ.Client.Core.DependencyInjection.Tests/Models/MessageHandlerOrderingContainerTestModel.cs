using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Models
{
    internal class MessageHandlerOrderingContainerTestModel
    {
        public IBaseMessageHandler MessageHandler { get; set; }

        public bool ShouldTrigger { get; set; }

        public int? OrderValue { get; set; }

        public int? CallOrder { get; set; }
    }
}