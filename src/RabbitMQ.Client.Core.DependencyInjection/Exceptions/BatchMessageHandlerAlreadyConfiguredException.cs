using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// An exception that is thrown when batch message handler of some type configured twice.
    /// </summary>
    public class BatchMessageHandlerAlreadyConfiguredException : Exception
    {
        /// <summary>
        /// Type of batch message handler.
        /// </summary>
        public Type BatchMessageHandlerType { get; }

        public BatchMessageHandlerAlreadyConfiguredException(Type type, string message) : base(message)
        {
            BatchMessageHandlerType = type;
        }
    }
}