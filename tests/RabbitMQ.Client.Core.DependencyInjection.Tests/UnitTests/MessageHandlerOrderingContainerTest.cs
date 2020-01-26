namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    internal class MessageHandlerOrderingContainerTest
    {
        public IBaseMessageHandler MessageHandler { get; set; }
        
        public bool ShouldTrigger { get; set; }
            
        public int? OrderValue { get; set; }
            
        public int? CallOrder { get; set; }
    }
}