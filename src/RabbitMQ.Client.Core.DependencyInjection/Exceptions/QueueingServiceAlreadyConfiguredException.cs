using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// An exception that is thrown when queuing service of the same type configured twice.
    /// </summary>
    public class QueueingServiceAlreadyConfiguredException : Exception
    {
        /// <summary>
        /// Type of a queuing service.
        /// </summary>
        public Type QueueingServiceType { get; }

        public QueueingServiceAlreadyConfiguredException(Type type, string message) : base(message)
        {
            QueueingServiceType = type;
        }
    }
}