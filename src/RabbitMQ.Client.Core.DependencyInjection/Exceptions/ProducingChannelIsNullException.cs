using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// An exception that is thrown during the publication of a message when the channel is null.
    /// </summary>
    public class ProducingChannelIsNullException : Exception
    {
        public ProducingChannelIsNullException(string message) : base(message)
        {
        }
    }
}