using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation
{
    internal static class AsyncEventingBasicConsumerValidationExtensions
    {
        internal static AsyncEventingBasicConsumer EnsureIsNotNull([NotNull]this AsyncEventingBasicConsumer? consumer)
        {
            if (consumer is null)
            {
                throw new ConsumerIsNullException();
            }

            return consumer;
        }
    }
}