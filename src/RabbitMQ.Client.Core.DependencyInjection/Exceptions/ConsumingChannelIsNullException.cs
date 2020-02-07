using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// An exception that is thrown during the process of starting a consumer when the channel is null.
    /// </summary>
    public class ConsumingChannelIsNullException : Exception
    {
        public ConsumingChannelIsNullException(string message) : base(message)
        {
        }
    }
}