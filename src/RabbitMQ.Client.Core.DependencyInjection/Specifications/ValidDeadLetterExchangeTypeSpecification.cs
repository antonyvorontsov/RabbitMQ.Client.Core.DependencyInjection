using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

[assembly:InternalsVisibleTo("RabbitMQ.Client.Core.DependencyInjection.Tests")]
namespace RabbitMQ.Client.Core.DependencyInjection.Specifications
{
    /// <summary>
    /// Specification that validates dead letter exchange types.
    /// </summary>
    /// <remarks>
    /// Dead letter exchange type is being validated only of RequeueFailedMessages is enabled.
    /// </remarks>
    internal class ValidDeadLetterExchangeTypeSpecification : Specification<RabbitMqExchangeOptions>
    {
        protected override Expression<Func<RabbitMqExchangeOptions, bool>> ToExpression()
        {
            var allowedExchangeTypes = ExchangeType.All()!;
            return options => !options.RequeueFailedMessages || allowedExchangeTypes.Contains(options.DeadLetterExchangeType);
        }
    }
}