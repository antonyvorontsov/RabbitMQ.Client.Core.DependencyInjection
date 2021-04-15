using System;
using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// This exception if throw when an exchange is being declared with a wrong type.
    /// </summary>
    public class InvalidExchangeTypeException : Exception
    {
        public InvalidExchangeTypeException(string exchangeName, string exchangeType)
        {
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            AllowedExchangeTypes = Client.ExchangeType.All();
        }

        /// <summary>
        /// Wrong exchange type.
        /// </summary>
        public string ExchangeType { get; }
        
        /// <summary>
        /// Name of the exchange that is being declared wrong.
        /// </summary>
        public string ExchangeName { get; }
        
        /// <summary>
        /// Allowed exchange types.
        /// </summary>
        public ICollection<string> AllowedExchangeTypes { get; }
    }
}