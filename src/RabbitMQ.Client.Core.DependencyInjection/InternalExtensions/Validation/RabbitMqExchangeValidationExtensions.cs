using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions.Validation
{
    internal static class RabbitMqExchangeValidationExtensions
    {
        internal static RabbitMqExchange EnsureIsNotNull([NotNull]this RabbitMqExchange? exchange)
        {
            if (exchange is null)
            {
                throw new ExchangeIsNullException();
            }

            return exchange;
        }
    }
}