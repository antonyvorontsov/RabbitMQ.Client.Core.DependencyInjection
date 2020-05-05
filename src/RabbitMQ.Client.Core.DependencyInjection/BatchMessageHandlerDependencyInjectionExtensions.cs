using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.InternalExtensions;
using RabbitMQ.Client.Core.DependencyInjection.Models;

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
            var configurationInstance = RabbitMqClientOptionsDependencyInjectionExtensions.GetRabbitMqClientOptionsInstance(configuration);
            services.ConfigureBatchConsumerConnectionOptions<TBatchMessageHandler>(configurationInstance);
            services.AddHostedService<TBatchMessageHandler>();
            return services;
        }

        /// <summary>
        /// Add batch message handler.
        /// </summary>
        /// <typeparam name="TBatchMessageHandler">Batch message handler type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <param name="configuration">RabbitMq configuration <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddBatchMessageHandler<TBatchMessageHandler>(this IServiceCollection services, RabbitMqClientOptions configuration)
            where TBatchMessageHandler : BaseBatchMessageHandler
        {
            CheckIfBatchMessageHandlerAlreadyConfigured<TBatchMessageHandler>(services);
            services.ConfigureBatchConsumerConnectionOptions<TBatchMessageHandler>(configuration);
            services.AddHostedService<TBatchMessageHandler>();
            return services;
        }

        static IServiceCollection ConfigureBatchConsumerConnectionOptions<TBatchMessageHandler>(this IServiceCollection services, RabbitMqClientOptions clientOptions)
            where TBatchMessageHandler : BaseBatchMessageHandler
        {
            var options = new BatchConsumerConnectionOptions
            {
                Type = typeof(TBatchMessageHandler),
                ClientOptions = clientOptions
            };
            var serviceDescriptor = new ServiceDescriptor(typeof(BatchConsumerConnectionOptions), options);
            services.Add(serviceDescriptor);
            return services;
        }

        static void CheckIfBatchMessageHandlerAlreadyConfigured<TBatchMessageHandler>(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(x => x.ImplementationType == typeof(TBatchMessageHandler));
            if (descriptor != null)
            {
                throw new BatchMessageHandlerAlreadyConfiguredException(typeof(TBatchMessageHandler), $"A batch message handler of type {typeof(TBatchMessageHandler)} has already been configured.");
            }
        }
    }
}