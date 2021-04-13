using System;
using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// Internal model that represents a dead letter exchange.
    /// </summary>
    internal sealed class DeadLetterExchange
    {
        internal DeadLetterExchange(string name, string type)
        {
            Name = name;
            Type = type;
        }
        
        /// <summary>
        /// Exchange name.
        /// </summary>
        internal string Name { get; }
        
        /// <summary>
        /// Exchange type.
        /// </summary>
        internal string Type { get; }
    }
    
    /// <summary>
    /// Default equality comparer for <see cref="DeadLetterExchange"/>.
    /// </summary>
    internal sealed class DeadLetterExchangeEqualityComparer : IEqualityComparer<DeadLetterExchange>
    {
        public bool Equals(DeadLetterExchange? x, DeadLetterExchange? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }
            
            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Type, y.Type, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(DeadLetterExchange obj)
        {
            return HashCode.Combine(obj.Name, obj.Type);
        }
    }
}