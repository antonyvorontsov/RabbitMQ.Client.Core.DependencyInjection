using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// This exception is thrown when consumer instance is null so there is no option of consuming messages.
    /// </summary>
    public class ConsumerIsNullException : Exception
    {
    }
}