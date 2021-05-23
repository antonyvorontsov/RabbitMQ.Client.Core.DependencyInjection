using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Exceptions
{
    /// <summary>
    /// This exception is thrown when the message has been acknowledged already.
    /// </summary>
    public class MessageHasAlreadyBeenAcknowledgedException : Exception
    {
    }
}