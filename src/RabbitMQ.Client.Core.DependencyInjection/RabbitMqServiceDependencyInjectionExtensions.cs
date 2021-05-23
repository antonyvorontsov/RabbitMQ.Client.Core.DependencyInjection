using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for RabbitMQ services (RabbitMQ connection).
    /// </summary>
    public static class RabbitMqServiceDependencyInjectionExtensions
    {
        /// <summary>
        /// Add fully-functional RabbitMQ services and required infrastructure.
        /// RabbitMQ services consists of two components: consumer <see cref="IConsumingService"/> and producer <see cref="IProducingService"/>.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <param name="behaviourConfiguration">Custom behaviour configuration options.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, IConfiguration configuration, Action<BehaviourConfiguration>? behaviourConfiguration = null)
        {
            services.AddBehaviourConfiguration(behaviourConfiguration);
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqConnectionOptions(RabbitMqServiceOptionsDependencyInjectionExtensions.GetRabbitMqServiceOptionsInstance(configuration));
            services.AddRabbitMqServices();
            services.AddConsumptionStarter();
            return services;
        }

        /// <summary>
        /// Add fully-functional RabbitMQ services and required infrastructure.
        /// RabbitMQ services consists of two components: consumer <see cref="IConsumingService"/> and producer <see cref="IProducingService"/>.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqServiceOptions"/>.</param>
        /// <param name="behaviourConfiguration">Custom behaviour configuration options.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, RabbitMqServiceOptions configuration, Action<BehaviourConfiguration>? behaviourConfiguration = null)
        {
            services.AddBehaviourConfiguration(behaviourConfiguration);
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqConnectionOptions(configuration);
            services.AddRabbitMqServices();
            services.AddConsumptionStarter();
            return services;
        }

        /// <summary>
        /// Add a singleton producing RabbitMQ service <see cref="IProducingService"/> and required infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <param name="behaviourConfiguration">Custom behaviour configuration options.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services, IConfiguration configuration, Action<BehaviourConfiguration>? behaviourConfiguration = null)
        {
            services.AddBehaviourConfiguration(behaviourConfiguration);
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqProducingOnlyServiceOptions(RabbitMqServiceOptionsDependencyInjectionExtensions.GetRabbitMqServiceOptionsInstance(configuration));
            services.AddRabbitMqServices();
            return services;
        }

        /// <summary>
        /// Add a singleton producing RabbitMQ service <see cref="IProducingService"/> and required infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqServiceOptions"/>.</param>
        /// <param name="behaviourConfiguration">Custom behaviour configuration options.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services, RabbitMqServiceOptions configuration, Action<BehaviourConfiguration>? behaviourConfiguration = null)
        {
            services.AddBehaviourConfiguration(behaviourConfiguration);
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqProducingOnlyServiceOptions(configuration);
            services.AddRabbitMqServices();
            return services;
        }

        private static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IProducingService, ProducingService>();
            services.TryAddSingleton<IConsumingService, ConsumingService>();
            return services;
        }

        private static IServiceCollection AddConsumptionStarter(this IServiceCollection services)
        {
            // TODO: change to TryAdd method
            services.AddHostedService<ConsumptionStarterHostedService>();
            return services;
        }

        private static IServiceCollection AddBehaviourConfiguration(this IServiceCollection services, Action<BehaviourConfiguration>? behaviourConfiguration = null)
        {
            var configuration = new BehaviourConfiguration();
            behaviourConfiguration?.Invoke(configuration);
            services.Configure<BehaviourConfiguration>(x => x.DisableInternalLogging = configuration.DisableInternalLogging);
            return services;
        }

        private static IServiceCollection AddRabbitMqInfrastructure(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
            services.TryAddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.TryAddSingleton<IMessageHandlerContainerBuilder, MessageHandlerContainerBuilder>();
            services.TryAddSingleton<IMessageHandlingPipelineExecutingService, MessageHandlingPipelineExecutingService>();
            services.TryAddSingleton<IMessageHandlingService, MessageHandlingService>();
            services.TryAddSingleton<IErrorProcessingService, ErrorProcessingService>();
            services.TryAddSingleton<IChannelDeclarationService, ChannelDeclarationService>();
            services.TryAddSingleton<ILoggingService, LoggingService>();
            // TODO: change to TryAdd method
            services.AddHostedService<ChannelDeclarationHostedService>();
            return services;
        }
    }
}