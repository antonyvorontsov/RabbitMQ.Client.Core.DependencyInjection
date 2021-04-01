using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Specifications
{
    /// <summary>
    /// Specification that represents rabbitMq exchange duplications.
    /// </summary>
    internal class DuplicatedRabbitMqExchangeDeclarationSpecification : Specification<ServiceDescriptor>
    {
        private readonly string _exchangeName;
        private readonly ClientExchangeType _exchangeType;

        public DuplicatedRabbitMqExchangeDeclarationSpecification(string exchangeName, ClientExchangeType exchangeType)
        {
            _exchangeName = exchangeName;
            _exchangeType = exchangeType;
        }

        protected override Expression<Func<ServiceDescriptor, bool>> ToExpression()
        {
            return x => x.ServiceType == typeof(RabbitMqExchange) &&
                x.Lifetime == ServiceLifetime.Singleton &&
                string.Equals(((ExchangeServiceDescriptor)x).ExchangeName, _exchangeName, StringComparison.OrdinalIgnoreCase) &&
                (_exchangeType == ((ExchangeServiceDescriptor)x).ClientExchangeType ||
                    ((ExchangeServiceDescriptor)x).ClientExchangeType == ClientExchangeType.Universal);
        }
    }
}