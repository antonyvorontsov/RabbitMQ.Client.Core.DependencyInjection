using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions
{
    /// <summary>
    /// Base DI extensions for all types of message handlers.
    /// </summary>
    internal static class BaseMessageHandlerDependencyInjectionExtensions
    {
        internal static IServiceCollection AddInstanceTransient<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where TInterface : class
            where TImplementation : class, TInterface =>
            services.AddInstanceTransient<TInterface, TImplementation>(routePatterns, null, order);

        internal static IServiceCollection AddInstanceSingleton<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where TInterface : class
            where TImplementation : class, TInterface =>
            services.AddInstanceSingleton<TInterface, TImplementation>(routePatterns, null, order);

        internal static IServiceCollection AddInstanceTransient<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var patterns = routePatterns.ToList();
            services.AddTransient<TInterface, TImplementation>();
            var router = new MessageHandlerRouter
            {
                Type = typeof(TImplementation),
                Exchange = exchange,
                RoutePatterns = patterns
            };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            services.AddMessageHandlerOrderingModel<TImplementation>(patterns, exchange, order);
            return services;
        }

        internal static IServiceCollection AddInstanceSingleton<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var patterns = routePatterns.ToList();
            services.AddSingleton<TInterface, TImplementation>();
            var router = new MessageHandlerRouter
            {
                Type = typeof(TImplementation),
                Exchange = exchange,
                RoutePatterns = patterns
            };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            services.AddMessageHandlerOrderingModel<TImplementation>(patterns, exchange, order);
            return services;
        }

        static IServiceCollection AddMessageHandlerOrderingModel<TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where TImplementation : class
        {
            var patterns = routePatterns.ToList();
            MessageHandlerOrderingModelExists<TImplementation>(services, patterns, exchange, order);
            var messageHandlerOrderingModel = new MessageHandlerOrderingModel
            {
                Exchange = exchange,
                RoutePatterns = patterns,
                Order = order,
                MessageHandlerType = typeof(TImplementation)
            };
            services.AddSingleton(messageHandlerOrderingModel);
            return services;
        }

        static void MessageHandlerOrderingModelExists<TImplementation>(IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
        {
            var patterns = routePatterns.ToList();
            var messageHandlerOrderingModel = services.FirstOrDefault(x => x.ServiceType == typeof(MessageHandlerOrderingModel)
                && x.Lifetime == ServiceLifetime.Singleton
                && ((MessageHandlerOrderingModel)x.ImplementationInstance).MessageHandlerType == typeof(TImplementation)
                && (string.Equals(((MessageHandlerOrderingModel)x.ImplementationInstance).Exchange, exchange, StringComparison.OrdinalIgnoreCase)
                    || (exchange is null && ((MessageHandlerOrderingModel)x.ImplementationInstance).Exchange is null))
                && ((MessageHandlerOrderingModel)x.ImplementationInstance).Order != order
                && patterns.Intersect(((MessageHandlerOrderingModel)x.ImplementationInstance).RoutePatterns).Any());
            if (messageHandlerOrderingModel is null)
            {
                return;
            }

            var intersectRoutePatterns = patterns.Intersect(((MessageHandlerOrderingModel)messageHandlerOrderingModel.ImplementationInstance).RoutePatterns);
            throw new ArgumentException($"A message handler {nameof(TImplementation)} for an exchange {exchange} has already been configured for route patterns[{string.Join(", ", intersectRoutePatterns)}] with an order {order}.");
        }
    }
}