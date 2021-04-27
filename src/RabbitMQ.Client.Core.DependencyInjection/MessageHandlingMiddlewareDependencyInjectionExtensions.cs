using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Middlewares;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for message handling middlewares that combine into single pipeline.
    /// </summary>
    public static class MessageHandlingMiddlewareDependencyInjectionExtensions
    {
        /// <summary>
        /// Add a middleware component that will be used in the message handling pipeline in a transient mode (by default).
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="T">Middleware type (has to implement <see cref="IMessageHandlingMiddleware"/>).</typeparam>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlingMiddleware<T>(this IServiceCollection services)
            where T : class, IMessageHandlingMiddleware =>
            services.AddMessageHandlingMiddlewareTransient<T>();

        /// <summary>
        /// Add a middleware component that will be used in the message handling pipeline in a singleton mode.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="T">Middleware type (has to implement <see cref="IMessageHandlingMiddleware"/>).</typeparam>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlingMiddlewareSingleton<T>(this IServiceCollection services)
            where T : class, IMessageHandlingMiddleware =>
            services.AddSingleton<IMessageHandlingMiddleware, T>();

        /// <summary>
        /// Add a middleware component that will be used in the message handling pipeline in a transient mode.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="T">Middleware type (has to implement <see cref="IMessageHandlingMiddleware"/>).</typeparam>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlingMiddlewareTransient<T>(this IServiceCollection services)
            where T : class, IMessageHandlingMiddleware => services.AddTransient<IMessageHandlingMiddleware, T>();
        
        /// <summary>
        /// Add a middleware component that will be used in the batch message handling pipeline in a transient mode (by default).
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="T">Middleware type (has to implement <see cref="IBatchMessageHandlingMiddleware"/>).</typeparam>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddBatchMessageHandlingMiddleware<T>(this IServiceCollection services)
            where T : class, IBatchMessageHandlingMiddleware =>
            services.AddBatchMessageHandlingMiddlewareTransient<T>();

        /// <summary>
        /// Add a middleware component that will be used in the batch message handling pipeline in a singleton mode.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="T">Middleware type (has to implement <see cref="IBatchMessageHandlingMiddleware"/>).</typeparam>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddBatchMessageHandlingMiddlewareSingleton<T>(this IServiceCollection services)
            where T : class, IBatchMessageHandlingMiddleware =>
            services.AddSingleton<IBatchMessageHandlingMiddleware, T>();

        /// <summary>
        /// Add a middleware component that will be used in the message handling pipeline in a singleton mode.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="T">Middleware type (has to implement <see cref="IBatchMessageHandlingMiddleware"/>).</typeparam>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddBatchMessageHandlingMiddlewareTransient<T>(this IServiceCollection services)
            where T : class, IBatchMessageHandlingMiddleware => services.AddTransient<IBatchMessageHandlingMiddleware, T>();
    }
}