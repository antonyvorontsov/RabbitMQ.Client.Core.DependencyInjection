using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

[assembly:InternalsVisibleTo("RabbitMQ.Client.Core.DependencyInjection.Tests")]
namespace RabbitMQ.Client.Core.DependencyInjection.Specifications
{
    /// <summary>
    /// Specification that validates exchange types.
    /// </summary>
    internal class ValidExchangeTypeSpecification : Specification<RabbitMqExchangeOptions>
    {
        protected override Expression<Func<RabbitMqExchangeOptions, bool>> ToExpression()
        {
            var allowedExchangeTypes = ExchangeType.All()!;
            return options => allowedExchangeTypes.Contains(options.Type);
        }
    }
}