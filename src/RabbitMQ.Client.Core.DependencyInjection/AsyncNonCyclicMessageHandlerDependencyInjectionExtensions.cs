using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.Extensions;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for async non-cyclic message handlers.
    /// </summary>
    public static class AsyncNonCyclicMessageHandlerDependencyInjectionExtensions
    {
        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), 0);

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), order);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), 0);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), order);

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
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, 0);

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, order);

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
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, 0);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, order);
    }
}