using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for message handler.
    /// </summary>
    public static class MessageHandlerDependencyInjectionExtensions
    {
        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routePattern) where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns) where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern) where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns) where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, string routePattern) where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns) where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern) where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns)  where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList());

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList());

        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(routePatterns.ToList(), exchange);

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(routePatterns.ToList(), exchange);

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routePatterns.ToList(), exchange);

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routePatterns.ToList(), exchange);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routePatterns.ToList(), exchange);

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routePatterns.ToList(), exchange);

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), exchange);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), exchange);

        static IServiceCollection AddInstanceTransient<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where TInterface : class
            where TImplementation : class, TInterface =>
            services.AddInstanceTransient<TInterface, TImplementation>(routePatterns, null);

        static IServiceCollection AddInstanceSingleton<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where TInterface : class
            where TImplementation : class, TInterface =>
            services.AddInstanceSingleton<TInterface, TImplementation>(routePatterns, null);

        static IServiceCollection AddInstanceTransient<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddTransient<TInterface, TImplementation>();
            var router = new MessageHandlerRouter
            {
                Type = typeof(TImplementation),
                Exchange = exchange,
                RoutePatterns = routePatterns.ToList()
            };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services;
        }

        static IServiceCollection AddInstanceSingleton<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddSingleton<TInterface, TImplementation>();
            var router = new MessageHandlerRouter
            {
                Type = typeof(TImplementation),
                Exchange = exchange,
                RoutePatterns = routePatterns.ToList()
            };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services;
        }
    }
}