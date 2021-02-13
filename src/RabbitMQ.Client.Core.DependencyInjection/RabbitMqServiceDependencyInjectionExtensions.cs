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
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqConnectionOptions(RabbitMqServiceOptionsDependencyInjectionExtensions.GetRabbitMqServiceOptionsInstance(configuration));
            services.AddProducer();
            services.AddConsumer();
            return services;
        }

        /// <summary>
        /// Add fully-functional RabbitMQ services and required infrastructure.
        /// RabbitMQ services consists of two components: consumer <see cref="IConsumingService"/> and producer <see cref="IProducingService"/>.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqServiceOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, RabbitMqServiceOptions configuration)
        {
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqConnectionOptions(configuration);
            services.AddProducer();
            services.AddConsumer();
            return services;
        }

        /// <summary>
        /// Add a singleton producing RabbitMQ service <see cref="IProducingService"/> and required infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqProducingServiceOptions(RabbitMqServiceOptionsDependencyInjectionExtensions.GetRabbitMqServiceOptionsInstance(configuration));
            services.AddProducer();
            return services;
        }

        /// <summary>
        /// Add a singleton producing RabbitMQ service <see cref="IProducingService"/> and required infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqServiceOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqProducer(this IServiceCollection services, RabbitMqServiceOptions configuration)
        {
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqProducingServiceOptions(configuration);
            services.AddProducer();
            return services;
        }

        /// <summary>
        /// Add a singleton consuming RabbitMQ service <see cref="IConsumingService"/> and required infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqConsumingServiceOptions(RabbitMqServiceOptionsDependencyInjectionExtensions.GetRabbitMqServiceOptionsInstance(configuration));
            services.AddConsumer();
            return services;
        }

        /// <summary>
        /// Add a singleton consuming RabbitMQ service <see cref="IConsumingService"/> and required infrastructure.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqServiceOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddRabbitMqConsumer(this IServiceCollection services, RabbitMqServiceOptions configuration)
        {
            services.AddRabbitMqInfrastructure();
            services.ConfigureRabbitMqConsumingServiceOptions(configuration);
            services.AddConsumer();
            return services;
        }

        private static IServiceCollection AddProducer(this IServiceCollection services)
        {
            services.TryAddSingleton<IProducingService, ProducingService>();
            return services;
        }

        private static IServiceCollection AddConsumer(this IServiceCollection services)
        {
            services.TryAddSingleton<IConsumingService, ConsumingService>();
            services.AddHostedService<ConsumingHostedService>();
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
            services.AddHostedService<ChannelDeclarationHostedService>();
            return services;
        }
    }
}