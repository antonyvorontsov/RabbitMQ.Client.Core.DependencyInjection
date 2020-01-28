using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class RabbitMqExchangeDependencyInjectionExtensionsTests
    {
        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameExchangeWithSameNameAndOptionsTwice()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                new ServiceCollection()
                    .AddExchange("exchange.name", true, new RabbitMqExchangeOptions())
                    .AddExchange("exchange.name", false, new RabbitMqExchangeOptions());
                return Task.CompletedTask;
            });
        }
        
        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameExchangeWithSameNameAndConfigurationTwice()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                var configurationMock = new Mock<IConfiguration>();
                new ServiceCollection()
                    .AddExchange("exchange.name", true, configurationMock.Object)
                    .AddExchange("exchange.name", false, configurationMock.Object);
                return Task.CompletedTask;
            });
        }
    }
}