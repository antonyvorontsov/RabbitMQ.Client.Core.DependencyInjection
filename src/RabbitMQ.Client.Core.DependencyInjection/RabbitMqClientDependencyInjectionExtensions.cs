using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for RabbitMQ client (RabbitMQ connection).
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
            services.AddSingleton<IMessageHandlingService, MessageHandlingService>();
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
                opt.HostNames = configuration.HostNames;
                opt.TcpEndpoints = configuration.TcpEndpoints;
                opt.Port = configuration.Port;
                opt.UserName = configuration.UserName;
                opt.Password = configuration.Password;
                opt.VirtualHost = configuration.VirtualHost;
                opt.AutomaticRecoveryEnabled = configuration.AutomaticRecoveryEnabled;
                opt.TopologyRecoveryEnabled = configuration.TopologyRecoveryEnabled;
                opt.RequestedConnectionTimeout = configuration.RequestedConnectionTimeout;
                opt.RequestedHeartbeat = configuration.RequestedHeartbeat;
                opt.ClientProvidedName = configuration.ClientProvidedName;
            });
            services.AddSingleton<IQueueService, QueueService>();
            return services;
        }
    }
}