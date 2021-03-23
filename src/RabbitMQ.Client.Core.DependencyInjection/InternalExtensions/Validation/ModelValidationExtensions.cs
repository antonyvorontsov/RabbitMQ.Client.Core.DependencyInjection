using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation
{
    internal static class ModelValidationExtensions
    {
        internal static IModel EnsureIsNotNull([NotNull]this IModel? channel)
        {
            if (channel is null)
            {
                throw new ChannelIsNullException();
            }

            return channel;
        }
    }
}