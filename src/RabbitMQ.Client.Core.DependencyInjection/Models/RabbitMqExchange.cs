using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// Exchange model.
    /// </summary>
    public class RabbitMqExchange
    {
        public RabbitMqExchange(string name, bool isConsuming, RabbitMqExchangeOptions options)
        {
            Name = name;
            IsConsuming = isConsuming;
            Options = options;
        }
        
        /// <summary>
        /// The unique name of the exchange.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag determining whether the exchange made for message consumption.
        /// If false then an exchange made only for publishing.
        /// </summary>
        public bool IsConsuming { get; set; }

        /// <summary>
        /// Exchange options.
        /// </summary>
        public RabbitMqExchangeOptions Options { get; set; }
    }
}