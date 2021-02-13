using System.Collections.Generic;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.IntegrationTests
{
    public class RabbitMqConnectionFactoryTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnection(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostName = "anotherHost",
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithConnectionName(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostName = "anotherHost",
                ClientProvidedName = "connectionName",
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithTcpEndpoints(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                TcpEndpoints = new List<RabbitMqTcpEndpoint>
                {
                    new()
                    {
                        HostName = "anotherHost"
                    }
                },
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithHostNames(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostNames = new List<string> { "anotherHost" },
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithHostNamesAndNamedConnection(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostNames = new List<string> { "anotherHost" },
                ClientProvidedName = "connectionName",
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnection()
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostName = "rabbitmq",
                InitialConnectionRetries = 1,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithConnectionName()
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostName = "rabbitmq",
                ClientProvidedName = "connectionName",
                InitialConnectionRetries = 3,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithTcpEndpoints()
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                TcpEndpoints = new List<RabbitMqTcpEndpoint>
                {
                    new()
                    {
                        HostName = "rabbitmq"
                    }
                },
                InitialConnectionRetries = 3,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithHostNames()
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostNames = new List<string> { "rabbitmq" },
                InitialConnectionRetries = 3,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithHostNamesAndNamedConnection()
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostNames = new List<string> { "rabbitmq" },
                ClientProvidedName = "connectionName",
                InitialConnectionRetries = 3,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        private static void ExecuteUnsuccessfulConnectionCreationAndAssertResults(RabbitMqServiceOptions connectionOptions)
        {
            var connectionFactory = new RabbitMqConnectionFactory();
            var exception = Assert.Throws<InitialConnectionException>(() => connectionFactory.CreateRabbitMqConnection(connectionOptions));
            Assert.Equal(connectionOptions.InitialConnectionRetries, exception.NumberOfRetries);
        }

        private static void ExecuteSuccessfulConnectionCreationAndAssertResults(RabbitMqServiceOptions connectionOptions)
        {
            var connectionFactory = new RabbitMqConnectionFactory();
            using var connection = connectionFactory.CreateRabbitMqConnection(connectionOptions);
            Assert.True(connection.IsOpen);
        }
    }
}