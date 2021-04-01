using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Specifications;

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

        internal static IServiceCollection AddInstanceTransient<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string? exchange, int order)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var patterns = routePatterns.ToList();
            services.AddTransient<TInterface, TImplementation>();
            var router = new MessageHandlerRouter(typeof(TImplementation), exchange, patterns);
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services.AddMessageHandlerOrderingModel<TImplementation>(patterns, exchange, order);
        }

        internal static IServiceCollection AddInstanceSingleton<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string? exchange, int order)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            var patterns = routePatterns.ToList();
            services.AddSingleton<TInterface, TImplementation>();
            var router = new MessageHandlerRouter(typeof(TImplementation), exchange, patterns);
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services.AddMessageHandlerOrderingModel<TImplementation>(patterns, exchange, order);
        }

        private static IServiceCollection AddMessageHandlerOrderingModel<TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string? exchange, int order)
            where TImplementation : class
        {
            var patterns = routePatterns.ToList();
            MessageHandlerOrderingModelExists<TImplementation>(services, patterns, exchange, order);
            var messageHandlerOrderingModel = new MessageHandlerOrderingModel(typeof(TImplementation), exchange, patterns, order);
            services.AddSingleton(messageHandlerOrderingModel);
            return services;
        }

        private static void MessageHandlerOrderingModelExists<TImplementation>(IServiceCollection services, IReadOnlyCollection<string> routePatterns, string? exchange, int order)
        {
            var specification = new DuplicatedMessageHandlerDeclarationSpecification(typeof(TImplementation), routePatterns, exchange, order);
            var messageHandlerOrderingModel = services.FirstOrDefault(x => specification.IsSatisfiedBy(x));
            if (messageHandlerOrderingModel is null)
            {
                return;
            }

            var intersectRoutePatterns = routePatterns.Intersect(((MessageHandlerOrderingModel)messageHandlerOrderingModel.ImplementationInstance).RoutePatterns);
            throw new ArgumentException($"A message handler {nameof(TImplementation)} for an exchange {exchange} has already been configured for route patterns[{string.Join(", ", intersectRoutePatterns)}] with an order {order}.");
        }
    }
}