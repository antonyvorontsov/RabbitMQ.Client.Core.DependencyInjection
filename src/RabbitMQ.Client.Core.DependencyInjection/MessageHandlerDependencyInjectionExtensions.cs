using System;
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
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routePattern)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(routePatterns.ToList(), 0);
        
        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(routePatterns.ToList(), order);

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(routePatterns.ToList(), 0);
        
        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(routePatterns.ToList(), order);

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, string routePattern)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routePatterns.ToList(), 0);

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routePatterns.ToList(), order);

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routePatterns.ToList(), 0);
        
        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routePatterns.ToList(), order);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routePatterns.ToList(), 0);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routePatterns.ToList(), order);

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), 0);

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routePatterns.ToList(), 0);

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), order);

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routePatterns.ToList(), order);

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
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceTransient<IMessageHandler, T>(routePatterns.ToList(), exchange, 0);
        
        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(routePatterns.ToList(), exchange, order);

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
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceSingleton<IMessageHandler, T>(routePatterns.ToList(), exchange, 0);
        
        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(routePatterns.ToList(), exchange, order);

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
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routePatterns.ToList(), exchange, 0);
        
        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routePatterns.ToList(), exchange, order);

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
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routePatterns.ToList(), exchange, 0);

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routePatterns.ToList(), exchange, order);
        
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
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, 0);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, order);
        
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
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, 0);

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
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, 0);
        
        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePattern">Route pattern.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routePattern, string exchange, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routePattern }.ToList(), exchange, order);

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routePatterns">Route patterns.</param>
        /// <param name="exchange">An exchange which will be "listened".</param>
        /// <param name="order">Message handler order.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
            where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routePatterns.ToList(), exchange, order);

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

        static IServiceCollection AddInstanceTransient<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where TInterface : class
            where TImplementation : class, TInterface =>
            services.AddInstanceTransient<TInterface, TImplementation>(routePatterns, null, order);

        static IServiceCollection AddInstanceSingleton<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, int order)
            where TInterface : class
            where TImplementation : class, TInterface =>
            services.AddInstanceSingleton<TInterface, TImplementation>(routePatterns, null, order);

        static IServiceCollection AddInstanceTransient<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
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
            services.AddMessageHandlerOrderingModel<TImplementation>(routePatterns, exchange, order);
            return services;
        }

        static IServiceCollection AddInstanceSingleton<TInterface, TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order)
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
            services.AddMessageHandlerOrderingModel<TImplementation>(routePatterns, exchange, order);
            return services;
        }
        
        static IServiceCollection AddMessageHandlerOrderingModel<TImplementation>(this IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order) 
            where TImplementation : class
        {
            MessageHandlerOrderingModelExists<TImplementation>(services, routePatterns, exchange, order);
            var messageHandlerOrderingModel = new MessageHandlerOrderingModel
            {
                Exchange = exchange,
                RoutePatterns = routePatterns,
                Order = order,
                MessageHandlerType = typeof(TImplementation)
            };
            services.AddSingleton(messageHandlerOrderingModel);
            return services;
        }

        static void MessageHandlerOrderingModelExists<TImplementation>(IServiceCollection services, IEnumerable<string> routePatterns, string exchange, int order) 
        {
            var messageHandlerOrderingModel = services.FirstOrDefault(x => x.ServiceType == typeof(MessageHandlerOrderingModel)
                && x.Lifetime == ServiceLifetime.Singleton
                && x.ImplementationType == typeof(TImplementation)
                && string.Equals(((MessageHandlerOrderingModel)x.ImplementationInstance).Exchange, exchange, StringComparison.OrdinalIgnoreCase)
                && ((MessageHandlerOrderingModel)x.ImplementationInstance).Order != order
                && routePatterns.Intersect(((MessageHandlerOrderingModel)x.ImplementationInstance).RoutePatterns).Any());
            if (messageHandlerOrderingModel is null)
            {
                return;
            }

            var intersectRoutePatterns = routePatterns.Intersect(((MessageHandlerOrderingModel)messageHandlerOrderingModel.ImplementationInstance).RoutePatterns);
            throw new ArgumentException($"A message handler {nameof(TImplementation)} for an exchange {exchange} has already been configured for route patterns[{string.Join(", ", intersectRoutePatterns)}] with an order {order}.");
        }
    }
}