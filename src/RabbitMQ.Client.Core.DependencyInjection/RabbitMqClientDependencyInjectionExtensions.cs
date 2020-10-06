using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for RabbitMQ client (RabbitMQ connection).
    /// </summary>
    public static class RabbitMqClientDependencyInjectionExtensions
    {
        /// <summary>
        /// Add a singleton fully-functional RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <remarks>
        /// QueueService will be added in the singleton mode.
        /// </remarks>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IQueueService>();
            services.AddRabbitMqClientInfrastructure();
            var configurationInstance = RabbitMqClientOptionsDependencyInjectionExtensions.GetRabbitMqClientOptionsInstance(configuration);
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConnectionOptions(guid, configurationInstance);
            services.ResolveSingletonQueueService(guid);
            return services;
        }

        /// <summary>
        /// Add a singleton fully-functional RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <remarks>
        /// QueueService will be added in the singleton mode.
        /// </remarks>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqClient(this IServiceCollection services, RabbitMqClientOptions configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IQueueService>();
            services.AddRabbitMqClientInfrastructure();
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConnectionOptions(guid, configuration);
            services.ResolveSingletonQueueService(guid);
            return services;
        }

        /// <summary>
        /// Add a transient fully-functional RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <remarks>
        /// QueueService will be added in the transient mode.
        /// </remarks>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqClientTransient(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IQueueService>();
            services.AddRabbitMqClientInfrastructure();
            var configurationInstance = RabbitMqClientOptionsDependencyInjectionExtensions.GetRabbitMqClientOptionsInstance(configuration);
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConnectionOptions(guid, configurationInstance);
            services.ResolveTransientQueueService(guid);
            return services;
        }

        /// <summary>
        /// Add a transient fully-functional RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <remarks>
        /// QueueService will be added in the transient mode.
        /// </remarks>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqClientTransient(this IServiceCollection services, RabbitMqClientOptions configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IQueueService>();
            services.AddRabbitMqClientInfrastructure();
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConnectionOptions(guid, configuration);
            services.ResolveTransientQueueService(guid);
            return services;
        }

        /// <summary>
        /// Add a singleton producing RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducingClientSingleton(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IProducingService>();
            services.AddRabbitMqClientInfrastructure();
            var configurationInstance = RabbitMqClientOptionsDependencyInjectionExtensions.GetRabbitMqClientOptionsInstance(configuration);
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqProducingClientOptions(guid, configurationInstance);
            services.ResolveSingletonProducingService(guid);
            return services;
        }

        /// <summary>
        /// Add a singleton producing RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducingClientSingleton(this IServiceCollection services, RabbitMqClientOptions configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IProducingService>();
            services.AddRabbitMqClientInfrastructure();
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqProducingClientOptions(guid, configuration);
            services.ResolveSingletonProducingService(guid);
            return services;
        }

        /// <summary>
        /// Add a transient producing RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducingClientTransient(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IProducingService>();
            services.AddRabbitMqClientInfrastructure();
            var configurationInstance = RabbitMqClientOptionsDependencyInjectionExtensions.GetRabbitMqClientOptionsInstance(configuration);
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqProducingClientOptions(guid, configurationInstance);
            services.ResolveTransientProducingService(guid);
            return services;
        }

        /// <summary>
        /// Add a singleton producing RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducingClientTransient(this IServiceCollection services, RabbitMqClientOptions configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IProducingService>();
            services.AddRabbitMqClientInfrastructure();
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqProducingClientOptions(guid, configuration);
            services.ResolveTransientProducingService(guid);
            return services;
        }

        /// <summary>
        /// Add a singleton consuming RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqConsumingClientSingleton(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IConsumingService>();
            services.AddRabbitMqClientInfrastructure();
            var configurationInstance = RabbitMqClientOptionsDependencyInjectionExtensions.GetRabbitMqClientOptionsInstance(configuration);
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConsumingClientOptions(guid, configurationInstance);
            services.ResolveSingletonConsumingService(guid);
            return services;
        }

        /// <summary>
        /// Add a singleton consuming RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqConsumingClientSingleton(this IServiceCollection services, RabbitMqClientOptions configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IConsumingService>();
            services.AddRabbitMqClientInfrastructure();
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConsumingClientOptions(guid, configuration);
            services.ResolveSingletonConsumingService(guid);
            return services;
        }

        /// <summary>
        /// Add a transient consuming RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqConsumingClientTransient(this IServiceCollection services, IConfiguration configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IConsumingService>();
            services.AddRabbitMqClientInfrastructure();
            var configurationInstance = RabbitMqClientOptionsDependencyInjectionExtensions.GetRabbitMqClientOptionsInstance(configuration);
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConsumingClientOptions(guid, configurationInstance);
            services.ResolveTransientConsumingService(guid);
            return services;
        }

        /// <summary>
        /// Add a transient consuming RabbitMQ client and required service infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqConsumingClientTransient(this IServiceCollection services, RabbitMqClientOptions configuration)
        {
            services.CheckIfQueueingServiceAlreadyConfigured<IConsumingService>();
            services.AddRabbitMqClientInfrastructure();
            var guid = Guid.NewGuid();
            services.ConfigureRabbitMqConsumingClientOptions(guid, configuration);
            services.ResolveTransientConsumingService(guid);
            return services;
        }

        static IServiceCollection AddRabbitMqClientInfrastructure(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
            services.TryAddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.TryAddSingleton<IMessageHandlerContainerBuilder, MessageHandlerContainerBuilder>();
            services.TryAddSingleton<IMessageHandlingService, MessageHandlingService>();
            return services;
        }

        static IServiceCollection ResolveSingletonQueueService(this IServiceCollection services, Guid guid)
        {
            services.TryAddSingleton<IQueueService>(provider => new QueueService(
                guid,
                provider.GetService<IRabbitMqConnectionFactory>(),
                provider.GetServices<RabbitMqConnectionOptionsContainer>(),
                provider.GetService<IMessageHandlingService>(),
                provider.GetServices<RabbitMqExchange>(),
                provider.GetService<ILogger<QueueService> >()));
            return services;
        }

        static IServiceCollection ResolveTransientQueueService(this IServiceCollection services, Guid guid)
        {
            services.TryAddTransient<IQueueService>(provider => new QueueService(
                guid,
                provider.GetService<IRabbitMqConnectionFactory>(),
                provider.GetServices<RabbitMqConnectionOptionsContainer>(),
                provider.GetService<IMessageHandlingService>(),
                provider.GetServices<RabbitMqExchange>(),
                provider.GetService<ILogger<QueueService> >()));
            return services;
        }

        static IServiceCollection ResolveSingletonProducingService(this IServiceCollection services, Guid guid)
        {
            services.TryAddSingleton<IProducingService>(provider => new QueueService(
                guid,
                provider.GetService<IRabbitMqConnectionFactory>(),
                provider.GetServices<RabbitMqConnectionOptionsContainer>(),
                provider.GetService<IMessageHandlingService>(),
                provider.GetServices<RabbitMqExchange>(),
                provider.GetService<ILogger<QueueService> >()));
            return services;
        }

        static IServiceCollection ResolveTransientProducingService(this IServiceCollection services, Guid guid)
        {
            services.TryAddTransient<IProducingService>(provider => new QueueService(
                guid,
                provider.GetService<IRabbitMqConnectionFactory>(),
                provider.GetServices<RabbitMqConnectionOptionsContainer>(),
                provider.GetService<IMessageHandlingService>(),
                provider.GetServices<RabbitMqExchange>(),
                provider.GetService<ILogger<QueueService> >()));
            return services;
        }

        static IServiceCollection ResolveSingletonConsumingService(this IServiceCollection services, Guid guid)
        {
            services.TryAddSingleton<IConsumingService>(provider => new QueueService(
                guid,
                provider.GetService<IRabbitMqConnectionFactory>(),
                provider.GetServices<RabbitMqConnectionOptionsContainer>(),
                provider.GetService<IMessageHandlingService>(),
                provider.GetServices<RabbitMqExchange>(),
                provider.GetService<ILogger<QueueService> >()));
            return services;
        }

        static IServiceCollection ResolveTransientConsumingService(this IServiceCollection services, Guid guid)
        {
            services.TryAddTransient<IConsumingService>(provider => new QueueService(
                guid,
                provider.GetService<IRabbitMqConnectionFactory>(),
                provider.GetServices<RabbitMqConnectionOptionsContainer>(),
                provider.GetService<IMessageHandlingService>(),
                provider.GetServices<RabbitMqExchange>(),
                provider.GetService<ILogger<QueueService> >()));
            return services;
        }

        static IServiceCollection CheckIfQueueingServiceAlreadyConfigured<T>(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(T));
            if (descriptor != null)
            {
                throw new QueueingServiceAlreadyConfiguredException(typeof(T), $"A queuing service of type {typeof(T)} has already been configured.");
            }

            return services;
        }
    }
}
