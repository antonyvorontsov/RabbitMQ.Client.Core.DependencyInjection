using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Класс с расширениями DI.
    /// </summary>
    public static class RabbitMqClientDependencyInjectionExtensions
    {
        /// <summary>
        /// Добавить RabbitMq клиента и все сопутствующие сервисы.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация подключения к серверу RabbitMq.</param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddLogging(options => options.AddConsole());
            services.Configure<RabbitMqClientOptions>(configuration);
            services.AddSingleton<IQueueService, QueueService>();
            return services;
        }

        /// <summary>
        /// Добавить точку обмена сообщений.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="exchangeName">Наименование точки обмена.</param>
        /// <param name="configuration">Конфигурация точки обмена.</param>
        /// <returns></returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, IConfiguration configuration)
        {
            var exchangeExists = services.Any(x => x.ServiceType == typeof(RabbitMqExchange)
                              && x.Lifetime == ServiceLifetime.Singleton
                              && string.Equals(((ExchangeServiceDescriptor)x).ExchangeName, exchangeName, StringComparison.OrdinalIgnoreCase));
            if (exchangeExists)
                throw new ArgumentException($"Exchange {exchangeName} has already been added!");

            var options = new RabbitMqExchangeOptions();
            configuration.Bind(options);
            var exchange = new RabbitMqExchange { Name = exchangeName, Options = options };
            var service = new ExchangeServiceDescriptor(typeof(RabbitMqExchange), exchange)
            {
                ExchangeName = exchangeName
            };
            services.Add(service);
            return services;
        }

        /// <summary>
        /// Добавить точку обмена сообщений.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="exchangeName">Наименование точки обмена.</param>
        /// <param name="options">Конфигурация точки обмена в виде объекта класса опций <see cref="RabbitMqExchangeOptions"/>.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddExchange(this IServiceCollection services, string exchangeName, RabbitMqExchangeOptions options = null)
        {
            var exchangeExists = services.Any(x => x.ServiceType == typeof(RabbitMqExchange)
                              && x.Lifetime == ServiceLifetime.Singleton
                              && string.Equals(((ExchangeServiceDescriptor)x).ExchangeName, exchangeName, StringComparison.OrdinalIgnoreCase));
            if (exchangeExists)
                throw new ArgumentException($"Exchange {exchangeName} has already been added!");

            var exchangeOptions = options ?? new RabbitMqExchangeOptions();
            var exchange = new RabbitMqExchange { Name = exchangeName, Options = exchangeOptions };
            var service = new ExchangeServiceDescriptor(typeof(RabbitMqExchange), exchange)
            {
                ExchangeName = exchangeName
            };
            services.Add(service);
            return services;
        }

        /// <summary>
        /// Добавить обработчик сообщений (reciever) в transient режиме.
        /// </summary>
        /// <typeparam name="T">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="routingKey">Ключ маршрутизации, на который подписан обработчик сообщений.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, string routingKey)
            where T : class, IMessageHandler, new()
        {
            var handler = new T { RoutingKeys = new[] { routingKey } };
            services.AddTransient<IMessageHandler, T>(_ => handler);
            return services;
        }

        /// <summary>
        /// Добавить обработчик сообщений (reciever) в transient режиме.
        /// </summary>
        /// <typeparam name="T">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="routingKeys">Коллекция ключей маршрутизации, на которые подписан обработчик сообщений.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddMessageHandlerTransient<T>(this IServiceCollection services, IEnumerable<string> routingKeys)
            where T : class, IMessageHandler, new()
        {
            var handler = new T { RoutingKeys = routingKeys };
            services.AddTransient<IMessageHandler, T>(_ => handler);
            return services;
        }

        /// <summary>
        /// Добавить обработчик сообщений (reciever) в singleton режиме.
        /// </summary>
        /// <typeparam name="T">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="routingKey">Ключ маршрутизации, на который подписан обработчик сообщений.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, string routingKey)
            where T : class, IMessageHandler, new()
        {
            var handler = new T { RoutingKeys = new[] { routingKey } };
            services.AddSingleton<IMessageHandler, T>(_ => handler);
            return services;
        }

        /// <summary>
        /// Добавить обработчик сообщений (reciever) в singleton режиме.
        /// </summary>
        /// <typeparam name="T">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="routingKeys">Коллекция ключей маршрутизации, на которые подписан обработчик сообщений.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddMessageHandlerSingleton<T>(this IServiceCollection services, IEnumerable<string> routingKeys)
            where T : class, IMessageHandler, new()
        {
            var handler = new T { RoutingKeys = routingKeys };
            services.AddSingleton<IMessageHandler, T>(_ => handler);
            return services;
        }

        /// <summary>
        /// Добавить обработчик сообщений (reciever) в scoped режиме.
        /// </summary>
        /// <typeparam name="T">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="routingKey">Ключ маршрутизации, на который подписан обработчик сообщений.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddMessageHandlerScoped<T>(this IServiceCollection services, string routingKey)
            where T : class, IMessageHandler, new()
        {
            var handler = new T { RoutingKeys = new[] { routingKey } };
            services.AddScoped<IMessageHandler, T>(_ => handler);
            return services;
        }

        /// <summary>
        /// Добавить обработчик сообщений (reciever) в scoped режиме.
        /// </summary>
        /// <typeparam name="T">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="routingKeys">Коллекция ключей маршрутизации, на которые подписан обработчик сообщений.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddMessageHandlerScoped<T>(this IServiceCollection services, IEnumerable<string> routingKeys)
            where T : class, IMessageHandler, new()
        {
            var handler = new T { RoutingKeys = routingKeys };
            services.AddScoped<IMessageHandler, T>(_ => handler);
            return services;
        }

        /// <summary>
        /// Добавить custom (клиентский) логгер для обработки сообщений.
        /// </summary>
        /// <typeparam name="T">Тип обработчика сообщений.</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection AddClientLogger<T>(this IServiceCollection services)
            where T : class, ILogger
        {
            services.AddSingleton<ILogger, T>();
            return services;
        }
    }
}