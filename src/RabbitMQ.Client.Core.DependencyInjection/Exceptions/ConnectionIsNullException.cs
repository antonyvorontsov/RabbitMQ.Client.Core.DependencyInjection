using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// This exception is thrown when connection is null (thus connection factory could not create a connection).
    /// </summary>
    public class ConnectionIsNullException : Exception
    {
    }
}