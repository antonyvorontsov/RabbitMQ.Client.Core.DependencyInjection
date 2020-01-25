namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// An internal model that contains information about message handler, its order and route by which it was matched with current message's routing key.
    /// </summary>
    internal class MessageHandlerOrderingContainer
    {
        /// <summary>
        /// The instance of message handler.
        /// </summary>
        public IBaseMessageHandler MessageHandler { get; set; }
        
        /// <summary>
        /// Order.
        /// </summary>
        public int? Order { get; set; }
        
        /// <summary>
        /// Route that matches a routing key of received message.
        /// </summary>
        public string MatchingRoute { get; set; }
    }
}