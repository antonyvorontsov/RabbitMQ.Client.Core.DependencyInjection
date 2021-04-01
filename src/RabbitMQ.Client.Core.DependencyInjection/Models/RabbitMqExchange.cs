using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// Exchange model.
    /// </summary>
    public class RabbitMqExchange
    {
        private readonly IReadOnlyCollection<ClientExchangeType> _exchangeTypesAllowedForConsuming = new[]
        {
            ClientExchangeType.Consumption,
            ClientExchangeType.Universal
        };
        
        public RabbitMqExchange(string name, ClientExchangeType clientExchangeType, RabbitMqExchangeOptions options)
        {
            Name = name;
            ClientExchangeType = clientExchangeType;
            Options = options;
        }
        
        /// <summary>
        /// The unique name of the exchange.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Custom client exchange type.
        /// </summary>
        public ClientExchangeType ClientExchangeType { get; }

        /// <summary>
        /// Flag determining whether the exchange made for message consumption.
        /// If false then an exchange made only for publishing.
        /// </summary>
        public bool IsConsuming => _exchangeTypesAllowedForConsuming.Contains(ClientExchangeType);

        /// <summary>
        /// Exchange options.
        /// </summary>
        public RabbitMqExchangeOptions Options { get; }
    }
}