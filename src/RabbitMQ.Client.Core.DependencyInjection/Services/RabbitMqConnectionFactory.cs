using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <summary>
    /// Service that is responsible for creating RabbitMQ connections depending on options <see cref="RabbitMqClientOptions"/>.
    /// </summary>
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        /// <summary>
        /// Create a RabbitMQ connection.
        /// </summary>
        /// <param name="options">An instance of options <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>An instance of connection <see cref="IConnection"/>.</returns>
        /// <remarks>If options parameter is null the method return null too.</remarks>
        public IConnection CreateRabbitMqConnection(RabbitMqClientOptions options)
        {
            if (options is null)
            {
                return null;
            }

            var factory = new ConnectionFactory
            {
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = options.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = options.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                RequestedHeartbeat = options.RequestedHeartbeat,
                DispatchConsumersAsync = true
            };

            if (options.TcpEndpoints?.Any() == true)
            {
                return CreateConnectionWithTcpEndpoints(options, factory);
            }

            return string.IsNullOrEmpty(options.ClientProvidedName)
                ? CreateConnection(options, factory)
                : CreateNamedConnection(options, factory);
        }

        /// <summary>
        /// Create a consumer depending on the connection channel.
        /// </summary>
        /// <param name="channel">Connection channel.</param>
        /// <returns>A consumer instance <see cref="AsyncEventingBasicConsumer"/>.</returns>
        public AsyncEventingBasicConsumer CreateConsumer(IModel channel) => new AsyncEventingBasicConsumer(channel);

        private static IConnection CreateConnectionWithTcpEndpoints(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            var clientEndpoints = new List<AmqpTcpEndpoint>();
            foreach (var endpoint in options.TcpEndpoints)
            {
                var sslOption = endpoint.SslOption;
                if (sslOption != null)
                {
                    var convertedOption = new SslOption(sslOption.ServerName, sslOption.CertificatePath, sslOption.Enabled);
                    if (!string.IsNullOrEmpty(sslOption.CertificatePassphrase))
                    {
                        convertedOption.CertPassphrase = sslOption.CertificatePassphrase;
                    }

                    if (sslOption.AcceptablePolicyErrors != null)
                    {
                        convertedOption.AcceptablePolicyErrors = sslOption.AcceptablePolicyErrors.Value;
                    }

                    clientEndpoints.Add(new AmqpTcpEndpoint(endpoint.HostName, endpoint.Port, convertedOption));
                }
                else
                {
                    clientEndpoints.Add(new AmqpTcpEndpoint(endpoint.HostName, endpoint.Port));
                }
            }
            return TryToCreateConnection(() => factory.CreateConnection(clientEndpoints), options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
        }

        private static IConnection CreateNamedConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return TryToCreateConnection(() => factory.CreateConnection(options.HostNames.ToList(), options.ClientProvidedName), options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
            }

            factory.HostName = options.HostName;
            return TryToCreateConnection(() => factory.CreateConnection(options.ClientProvidedName), options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
        }

        private static IConnection CreateConnection(RabbitMqClientOptions options, ConnectionFactory factory)
        {
            if (options.HostNames?.Any() == true)
            {
                return TryToCreateConnection(() => factory.CreateConnection(options.HostNames.ToList()), options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
            }

            factory.HostName = options.HostName;
            return TryToCreateConnection(factory.CreateConnection, options.InitialConnectionRetries, options.InitialConnectionRetryTimeoutMilliseconds);
        }

        private static IConnection TryToCreateConnection(Func<IConnection> connectionFunction, int numberOfRetries, int timeoutMilliseconds)
        {
            ValidateArguments(numberOfRetries, timeoutMilliseconds);

            var attempts = 0;
            BrokerUnreachableException latestException = null;
            while (attempts < numberOfRetries)
            {
                try
                {
                    if (attempts > 0)
                    {
                        Thread.Sleep(timeoutMilliseconds);
                    }

                    return connectionFunction();
                }
                catch (BrokerUnreachableException exception)
                {
                    attempts++;
                    latestException = exception;
                }
            }

            throw new InitialConnectionException($"Could not establish an initial connection in {numberOfRetries} retries", latestException)
            {
                NumberOfRetries = attempts
            };
        }

        private static void ValidateArguments(int numberOfRetries, int timeoutMilliseconds)
        {
            if (numberOfRetries < 1)
            {
                throw new ArgumentException("Number of retries should be a positive number.", nameof(numberOfRetries));
            }

            if (timeoutMilliseconds < 1)
            {
                throw new ArgumentException("Initial reconnection timeout should be a positive number.", nameof(timeoutMilliseconds));
            }
        }
    }
}