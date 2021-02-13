namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// An options model that "contains" sections for producing and consuming connections of a RabbitMQ clients.
    /// </summary>
    public class RabbitMqConnectionOptions
    {
        /// <summary>
        /// Producer connection.
        /// </summary>
        public RabbitMqServiceOptions ProducerOptions { get; set; }

        /// <summary>
        /// Consumer connection.
        /// </summary>
        public RabbitMqServiceOptions ConsumerOptions { get; set; }
    }
}