using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using System;
using System.Linq;

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
            services.AddExchange(exchangeName, isConsuming: true, configuration);

        /// <summary>
        /// Add a production exchange as singleton.
        /// Production exchange made only for producing messages into queues and cannot consume at all.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="configuration">Exchange configuration section.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddProductionExchange(this IServiceCollection services, string exchangeName, IConfiguration configuration) =>
            services.AddExchange(exchangeName, isConsuming: false, configuration);

        /// <summary>
        /// Add a consumption exchange as singleton.
        /// Consumption exchange can be used for producing messages as well as for consuming.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="options">Exchange configuration <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddConsumptionExchange(this IServiceCollection services, string exchangeName, RabbitMqExchangeOptions options) =>
            services.AddExchange(exchangeName, isConsuming: true, options);

        /// <summary>
        /// Add a production exchange as singleton.
        /// Production exchange made only for producing messages into queues and cannot consume at all.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="options">Exchange configuration <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddProductionExchange(this IServiceCollection services, string exchangeName, RabbitMqExchangeOptions options) =>
            services.AddExchange(exchangeName, isConsuming: false, options);

        /// <summary>
        /// Add an exchange as a singleton.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="configuration">Exchange configuration section.</param>
        /// <param name="isConsuming">Flag whether an exchange made for consumption.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, bool isConsuming, IConfiguration configuration)
        {
            CheckExchangeExists(services, exchangeName);

            var options = new RabbitMqExchangeOptions();
            configuration.Bind(options);
            return services.AddExchange(exchangeName, isConsuming, options);
        }

        /// <summary>
        /// Add an exchange as a singleton.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="options">Exchange configuration <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <param name="isConsuming">Flag whether an exchange made for consumption.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, bool isConsuming, RabbitMqExchangeOptions options)
        {
            CheckExchangeExists(services, exchangeName);

            var exchangeOptions = options ?? new RabbitMqExchangeOptions();
            var exchange = new RabbitMqExchange
            {
                Name = exchangeName,
                IsConsuming = isConsuming,
                Options = exchangeOptions
            };
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
                throw new ArgumentException($"Exchange {exchangeName} has been added already!");
        }
    }
}