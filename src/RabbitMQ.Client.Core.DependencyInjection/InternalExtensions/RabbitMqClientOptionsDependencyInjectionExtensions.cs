using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions
{
    /// <summary>
    /// Base DI extensions for RabbitMQ configuration options.
    /// </summary>
    internal static class RabbitMqClientOptionsDependencyInjectionExtensions
    {
        internal static RabbitMqClientOptions GetRabbitMqClientOptionsInstance(IConfiguration configuration)
        {
            var options = new RabbitMqClientOptions();
            configuration.Bind(options);
            return options;
        }

        internal static IServiceCollection ConfigureRabbitMqProducingClientOptions(this IServiceCollection services, Guid guid, RabbitMqClientOptions options)
        {
            var container = new RabbitMqConnectionOptionsContainer
            {
                Guid = guid,
                Options = new RabbitMqConnectionOptions { ProducerOptions = options }
            };
            return services.AddRabbitMqConnectionOptionsContainer(container);
        }

        internal static IServiceCollection ConfigureRabbitMqConsumingClientOptions(this IServiceCollection services, Guid guid, RabbitMqClientOptions options)
        {
            var container = new RabbitMqConnectionOptionsContainer
            {
                Guid = guid,
                Options = new RabbitMqConnectionOptions { ConsumerOptions = options }
            };
            return services.AddRabbitMqConnectionOptionsContainer(container);
        }

        internal static IServiceCollection ConfigureRabbitMqConnectionOptions(this IServiceCollection services, Guid guid, RabbitMqClientOptions options)
        {
            var container = new RabbitMqConnectionOptionsContainer
            {
                Guid = guid,
                Options = new RabbitMqConnectionOptions
                {
                    ProducerOptions = options,
                    ConsumerOptions = options
                }
            };
            return services.AddRabbitMqConnectionOptionsContainer(container);
        }

        static IServiceCollection AddRabbitMqConnectionOptionsContainer(this IServiceCollection services, RabbitMqConnectionOptionsContainer container)
        {
            var serviceDescriptor = new ServiceDescriptor(typeof(RabbitMqConnectionOptionsContainer), container);
            services.Add(serviceDescriptor);
            return services;
        }
    }
}