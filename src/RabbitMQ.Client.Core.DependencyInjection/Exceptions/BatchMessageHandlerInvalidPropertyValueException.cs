using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// An exception that is thrown when batch message handler implementation has invalid value for a property.
    /// </summary>
    public class BatchMessageHandlerInvalidPropertyValueException : Exception
    {
        /// <summary>
        /// Property that contains invalid value.
        /// </summary>
        public string PropertyName { get; set; }
        
        public BatchMessageHandlerInvalidPropertyValueException(string message, string propertyName) : base(message)
        {
            PropertyName = propertyName;
        }
    }
}