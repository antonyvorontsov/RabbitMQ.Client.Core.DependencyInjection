using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation
{
    internal static class ConnectionValidationExtensions
    {
        internal static IConnection EnsureIsNotNull([NotNull] this IConnection? connection)
        {
            if (connection is null)
            {
                throw new ConnectionIsNullException();
            }

            return connection;
        }
    }
}