using System;
using System.Linq;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Service that contains business logic of creating RabbitMQ connections depending on options <see cref="RabbitMqClientOptions"/>.
    /// </summary>
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        readonly RabbitMqClientOptions _options;
        
        public RabbitMqConnectionFactory(IOptions<RabbitMqClientOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentException($"Argument {nameof(options)} is null.", nameof(options));
            }
            
            _options = options.Value;
        }

        /// <summary>
        /// Create a RabbitMQ connection.
        /// </summary>
        /// <returns>An instance of connection <see cref="IConnection"/>.</returns>
        public IConnection CreateRabbitMqConnection()
        {
            var factory = new ConnectionFactory
            {
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = _options.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = _options.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = _options.RequestedConnectionTimeout,
                RequestedHeartbeat = _options.RequestedHeartbeat,
                DispatchConsumersAsync = true
            };

            if (_options.TcpEndpoints?.Any() == true)
            {
                var clientEndpoints = _options.TcpEndpoints.Select(x => new AmqpTcpEndpoint(x.HostName, x.Port)).ToList();
                return factory.CreateConnection(clientEndpoints);
            }

            return string.IsNullOrEmpty(_options.ClientProvidedName)
                ? CreateConnection(_options, factory)
                : CreateNamedConnection(_options, factory);
        }

        static IConnection CreateNamedConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return factory.CreateConnection(options.HostNames.ToList(), options.ClientProvidedName);
            }

            factory.HostName = options.HostName;
            return factory.CreateConnection(options.ClientProvidedName);
        }

        static IConnection CreateConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return factory.CreateConnection(options.HostNames.ToList());
            }

            factory.HostName = options.HostName;
            return factory.CreateConnection();
        }
    }
}