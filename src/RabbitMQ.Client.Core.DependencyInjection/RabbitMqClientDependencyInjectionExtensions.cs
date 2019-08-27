using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions.
    /// </summary>
    public static class RabbitMqClientDependencyInjectionExtensions
    {
        /// <summary>
        /// Add RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddLogging(options => options.AddConsole());
            services.Configure<RabbitMqClientOptions>(configuration);
            services.AddSingleton<IQueueService, QueueService>();
            return services;
        }

        /// <summary>
        /// Add RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqClient(this IServiceCollection services, RabbitMqClientOptions configuration)
        {
            services.AddLogging(options => options.AddConsole());
            services.Configure<RabbitMqClientOptions>(opt => 
            {
                opt.HostName = configuration.HostName;
                opt.Port = configuration.Port;
                opt.UserName = configuration.UserName;
                opt.Password = configuration.Password;
                opt.VirtualHost = configuration.VirtualHost;
                opt.AutomaticRecoveryEnabled = configuration.AutomaticRecoveryEnabled;
                opt.TopologyRecoveryEnabled = configuration.TopologyRecoveryEnabled;
                opt.RequestedConnectionTimeout = configuration.RequestedConnectionTimeout;
                opt.RequestedHeartbeat = configuration.RequestedHeartbeat;
            });
            services.AddSingleton<IQueueService, QueueService>();
            return services;
        }

        /// <summary>
        /// Add exchange as singleton.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="configuration">Exchange configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, IConfiguration configuration)
        {
            CheckExchangeExists(services, exchangeName);

            var options = new RabbitMqExchangeOptions();
            configuration.Bind(options);
            return services.AddExchange(exchangeName, options);
        }

        /// <summary>
        /// Add exchange as singleton.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="options">Exchange configuration <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, RabbitMqExchangeOptions options)
        {
            CheckExchangeExists(services, exchangeName);

            var exchangeOptions = options ?? new RabbitMqExchangeOptions();
            var exchange = new RabbitMqExchange { Name = exchangeName, Options = exchangeOptions };
            var service = new ExchangeServiceDescriptor(typeof(RabbitMqExchange), exchange)
            {
                ExchangeName = exchangeName
            };
            services.Add(service);
            return services;
        }

        static void CheckExchangeExists(IServiceCollection services, string exchangeName)
        {
            var exchangeExists = services.Any(x => x.ServiceType == typeof(RabbitMqExchange)
                              && x.Lifetime == ServiceLifetime.Singleton
                              && string.Equals(((ExchangeServiceDescriptor)x).ExchangeName, exchangeName, StringComparison.OrdinalIgnoreCase));
            if (exchangeExists)
                throw new ArgumentException($"Exchange {exchangeName} has already been added!");
        }

        /// <summary>
        /// Add transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add transient message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IMessageHandler =>
            services.AddInstanceTransient<IMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add singleton message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IMessageHandler =>
            services.AddInstanceSingleton<IMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add transient async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IAsyncMessageHandler =>
            services.AddInstanceTransient<IAsyncMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add singleton async message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys)  where T : class, IAsyncMessageHandler =>
            services.AddInstanceSingleton<IAsyncMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add transient non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceTransient<INonCyclicMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add singleton non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, INonCyclicMessageHandler =>
            services.AddInstanceSingleton<INonCyclicMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add transient async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKeys">Routing keys.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceTransient<IAsyncNonCyclicMessageHandler, T>(routingKeys.ToList());

        /// <summary>
        /// Add singleton async non-cyclic message handler.
        /// </summary>
        /// <typeparam name="T">Message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddAsyncNonCyclicMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey) where T : class, IAsyncNonCyclicMessageHandler =>
            services.AddInstanceSingleton<IAsyncNonCyclicMessageHandler, T>(new[] { routingKey }.ToList());

        /// <summary>
        /// Add singleton async non-cyclic message handler.
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
            var router = new MessageHandlerRouter { Type = typeof(T), RoutingKeys = routingKeys.ToList() };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services;
        }

        static IServiceCollection AddInstanceSingleton<U, T>(this IServiceCollection services, IEnumerable<string> routingKeys)
            where U : class
            where T : class, U
        {
            services.AddSingleton<U, T>();
            var router = new MessageHandlerRouter { Type = typeof(T), RoutingKeys = routingKeys.ToList() };
            services.Add(new ServiceDescriptor(typeof(MessageHandlerRouter), router));
            return services;
        }
    }
}