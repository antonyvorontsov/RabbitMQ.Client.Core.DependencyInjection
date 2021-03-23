using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation
{
    internal static class ModelValidationExtensions
    {
        internal static IModel EnsureIsNotNull([NotNull]this IModel? channel)
        {
            if (channel is null)
            {
                // TODO: change exception.
                throw new System.Exception();
            }

            return channel;
        }
    }

    internal static class ConnectionValidationExtensions
    {
        internal static IConnection EnsureIsNotNull([NotNull] this IConnection? connection)
        {
            if (connection is null)
            {
                // TODO: change exception.
                throw new System.Exception();
            }

            return connection;
        }
    }

    internal static class AsyncEventingBasicConsumerValidationExtensions
    {
        internal static AsyncEventingBasicConsumer EnsureIsNotNull([NotNull]this AsyncEventingBasicConsumer? consumer)
        {
            if (consumer is null)
            {
                // TODO: change exception.
                throw new System.Exception();
            }

            return consumer;
        }
    }
}