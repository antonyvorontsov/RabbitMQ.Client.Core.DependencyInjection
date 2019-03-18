using RabbitMQ.Client.Core.Configuration;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Exchange model.
    /// </summary>
    public class RabbitMqExchange
    {
        /// <summary>
        /// The unique name of the exchange.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Exchange options.
        /// </summary>
        public RabbitMqExchangeOptions Options { get; set; }
    }
}