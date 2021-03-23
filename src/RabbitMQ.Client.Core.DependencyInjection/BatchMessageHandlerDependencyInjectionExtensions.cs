using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for batch message handlers.
    /// </summary>
    public static class BatchMessageHandlerDependencyInjectionExtensions
    {
        /// <summary>
        /// Add batch message handler.
        /// </summary>
        /// <typeparam name="TBatchMessageHandler">Batch message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddBatchMessageHandler<TBatchMessageHandler>(this IServiceCollection services, IConfiguration configuration)
            where TBatchMessageHandler : BaseBatchMessageHandler
        {
            CheckIfBatchMessageHandlerAlreadyConfigured<TBatchMessageHandler>(services);
            services.TryAddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            var configurationInstance = RabbitMqServiceOptionsDependencyInjectionExtensions.GetRabbitMqServiceOptionsInstance(configuration);
            services.ConfigureBatchConsumerConnectionOptions<TBatchMessageHandler>(configurationInstance);
            services.AddHostedService<TBatchMessageHandler>();
            return services;
        }

        /// <summary>
        /// Add batch message handler.
        /// </summary>
        /// <typeparam name="TBatchMessageHandler">Batch message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqServiceOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddBatchMessageHandler<TBatchMessageHandler>(this IServiceCollection services, RabbitMqServiceOptions configuration)
            where TBatchMessageHandler : BaseBatchMessageHandler
        {
            CheckIfBatchMessageHandlerAlreadyConfigured<TBatchMessageHandler>(services);
            services.TryAddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.ConfigureBatchConsumerConnectionOptions<TBatchMessageHandler>(configuration);
            services.AddHostedService<TBatchMessageHandler>();
            return services;
        }

        private static IServiceCollection ConfigureBatchConsumerConnectionOptions<TBatchMessageHandler>(this IServiceCollection services, RabbitMqServiceOptions serviceOptions)
            where TBatchMessageHandler : BaseBatchMessageHandler
        {
            var options = new BatchConsumerConnectionOptions(typeof(TBatchMessageHandler), serviceOptions);
            var serviceDescriptor = new ServiceDescriptor(typeof(BatchConsumerConnectionOptions), options);
            services.Add(serviceDescriptor);
            return services;
        }

        private static void CheckIfBatchMessageHandlerAlreadyConfigured<TBatchMessageHandler>(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(x => x.ImplementationType == typeof(TBatchMessageHandler));
            if (descriptor != null)
            {
                throw new BatchMessageHandlerAlreadyConfiguredException(typeof(TBatchMessageHandler), $"A batch message handler of type {typeof(TBatchMessageHandler)} has already been configured.");
            }
        }
    }
}