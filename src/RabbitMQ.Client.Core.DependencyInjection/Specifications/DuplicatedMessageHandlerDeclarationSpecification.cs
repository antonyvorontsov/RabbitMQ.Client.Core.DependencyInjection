using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Specifications
{
    /// <summary>
    /// Specification that represents duplication of same message handlers with different orders.
    /// </summary>
    internal class DuplicatedMessageHandlerDeclarationSpecification : Specification<ServiceDescriptor>
    {
        private readonly Type _implementationType;
        private readonly IReadOnlyCollection<string> _routePatterns;
        private readonly string? _exchange;
        private readonly int _order;

        internal DuplicatedMessageHandlerDeclarationSpecification(
            Type implementationType,
            IReadOnlyCollection<string> routePatterns,
            string? exchange,
            int order)
        {
            _implementationType = implementationType;
            _routePatterns = routePatterns;
            _exchange = exchange;
            _order = order;
        }

        protected override Expression<Func<ServiceDescriptor, bool>> ToExpression()
        {
            return x => x.ServiceType == typeof(MessageHandlerOrderingModel) &&
                x.Lifetime == ServiceLifetime.Singleton &&
                ((MessageHandlerOrderingModel)x.ImplementationInstance).MessageHandlerType == _implementationType &&
                (string.Equals(((MessageHandlerOrderingModel)x.ImplementationInstance).Exchange, _exchange, StringComparison.OrdinalIgnoreCase) ||
                    (_exchange != null && ((MessageHandlerOrderingModel)x.ImplementationInstance).Exchange != null)) &&
                (((MessageHandlerOrderingModel)x.ImplementationInstance)!).Order != _order &&
                _routePatterns.Intersect(((MessageHandlerOrderingModel)x.ImplementationInstance).RoutePatterns).Any();
        }
    }
}