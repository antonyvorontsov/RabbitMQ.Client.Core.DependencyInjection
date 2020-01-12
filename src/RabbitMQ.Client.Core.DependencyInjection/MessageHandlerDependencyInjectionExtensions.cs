using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys)  where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add a singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(routingKeys.ToList());
        
        static IServiceCollection AddInstanceTransient<U, T>(this IServiceCollection services, IEnumerable<string> routingKeys)
            where U : class
            where T : class, U
        {
            services.AddTransient<U, T>();
            var router = new MessageHandlerRouter { Type = typeof(T), RoutePatterns = routingKeys.ToList() };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services;
        }

        static IServiceCollection AddInstanceSingleton<U, T>(this IServiceCollection services, IEnumerable<string> routingKeys)
            where U : class
            where T : class, U
        {
            services.AddSingleton<U, T>();
            var router = new MessageHandlerRouter { Type = typeof(T), RoutePatterns = routingKeys.ToList() };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services;
        }
    }
}