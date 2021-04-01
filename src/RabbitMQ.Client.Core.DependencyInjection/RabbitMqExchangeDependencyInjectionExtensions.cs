using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using System;
using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Specifications;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for RabbitMQ exchange.
    /// </summary>
    public static class RabbitMqExchangeDependencyInjectionExtensions
    {
        /// <summary>
        /// Add a consumption exchange as singleton.
        /// Consumption exchange can be used for producing messages as well as for consuming.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="configuration">Exchange configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddConsumptionExchange(this IServiceCollection services, string exchangeName, IConfiguration configuration) =>
            services.AddExchange(exchangeName, configuration, ClientExchangeType.Consumption);

        /// <summary>
        /// Add a production exchange as singleton.
        /// Production exchange made only for producing messages into queues and cannot consume at all.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="configuration">Exchange configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddProductionExchange(this IServiceCollection services, string exchangeName, IConfiguration configuration) =>
            services.AddExchange(exchangeName, configuration, ClientExchangeType.Production);

        /// <summary>
        /// Add a consumption exchange as singleton.
        /// Consumption exchange can be used for producing messages as well as for consuming.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="options">Exchange configuration <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddConsumptionExchange(this IServiceCollection services, string exchangeName, RabbitMqExchangeOptions options) =>
            services.AddExchange(exchangeName, options, ClientExchangeType.Consumption);

        /// <summary>
        /// Add a production exchange as singleton.
        /// Production exchange made only for producing messages into queues and cannot consume at all.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="options">Exchange configuration <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddProductionExchange(this IServiceCollection services, string exchangeName, RabbitMqExchangeOptions options) =>
            services.AddExchange(exchangeName, options, ClientExchangeType.Production);

        /// <summary>
        /// Add an exchange as a singleton.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="configuration">Exchange configuration section.</param>
        /// <param name="clientExchangeType">Custom client exchange type that defines what functionality is allowed for an exchange.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, IConfiguration configuration, ClientExchangeType clientExchangeType = ClientExchangeType.Universal)
        {
            CheckExchangeExists(services, exchangeName, clientExchangeType);

            var options = new RabbitMqExchangeOptions();
            configuration.Bind(options);
            return services.AddExchange(exchangeName, options, clientExchangeType);
        }

        /// <summary>
        /// Add an exchange as a singleton.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="options">Exchange configuration <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <param name="clientExchangeType">Custom client exchange type that defines what functionality is allowed for an exchange.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, RabbitMqExchangeOptions? options, ClientExchangeType clientExchangeType = ClientExchangeType.Universal)
        {
            CheckExchangeExists(services, exchangeName, clientExchangeType);

            var exchangeOptions = options ?? new RabbitMqExchangeOptions();
            var exchange = new RabbitMqExchange(exchangeName, clientExchangeType, exchangeOptions);
            var service = new ExchangeServiceDescriptor(typeof(RabbitMqExchange), exchange, exchangeName, clientExchangeType);
            services.Add(service);
            return services;
        }

        private static void CheckExchangeExists(IServiceCollection services, string exchangeName, ClientExchangeType clientExchangeType)
        {
            var specification = new DuplicatedRabbitMqExchangeDeclarationSpecification(exchangeName, clientExchangeType);
            if (services.Any(x => specification.IsSatisfiedBy(x)))
            {
                throw new ArgumentException($"Exchange {exchangeName} has been added already!");
            }
        }
    }
}