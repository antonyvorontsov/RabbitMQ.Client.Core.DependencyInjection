using System;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// A service extension for registration exchange singleton "services".
    /// </summary>
    internal class ExchangeServiceDescriptor : ServiceDescriptor
    {
        /// <summary>
        /// Name of the exchange that has been registered.
        /// </summary>
        public string ExchangeName { get; }
        
        /// <summary>
        /// Type of the exchange.
        /// </summary>
        public ClientExchangeType ClientExchangeType { get; }

        public ExchangeServiceDescriptor(Type serviceType, object instance, string exchangeName, ClientExchangeType clientExchangeType)
            : base(serviceType, instance)
        {
            ExchangeName = exchangeName;
            ClientExchangeType = clientExchangeType;
        }
    }
}