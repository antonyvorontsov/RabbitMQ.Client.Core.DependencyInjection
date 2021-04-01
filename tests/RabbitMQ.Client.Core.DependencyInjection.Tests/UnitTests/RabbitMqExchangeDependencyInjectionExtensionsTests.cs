using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class RabbitMqExchangeDependencyInjectionExtensionsTests
    {
        [Fact]
        public void ShouldProperlyThrowExceptionWhenRegisteringSameExchangeOfSameTypeTwice()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    new ServiceCollection()
                        .AddExchange("exchange.name", new RabbitMqExchangeOptions(), ClientExchangeType.Consumption)
                        .AddExchange("exchange.name", new RabbitMqExchangeOptions(), ClientExchangeType.Consumption);
                });
        }
        
        [Fact]
        public void ShouldProperlyNotThrowExceptionWhenRegisteringSameExchangeWithDifferentTypeTwice()
        {
            new ServiceCollection()
                .AddConsumptionExchange("exchange.name", new RabbitMqExchangeOptions())
                .AddProductionExchange("exchange.name", new RabbitMqExchangeOptions());
        }

        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameExchangeWithSameNameAndConfigurationTwice()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                var configurationMock = new Mock<IConfiguration>();
                new ServiceCollection()
                    .AddExchange("exchange.name", configurationMock.Object, ClientExchangeType.Consumption)
                    .AddExchange("exchange.name", configurationMock.Object, ClientExchangeType.Consumption);
                return Task.CompletedTask;
            });
        }
        
        [Fact]
        public void ShouldProperlyNotThrowExceptionWhenRegisteringSameExchangeWithDifferentTypeButSameConfigurationTwice()
        {
            var configurationMock = new Mock<IConfiguration>();
            new ServiceCollection()
                .AddConsumptionExchange("exchange.name", configurationMock.Object)
                .AddProductionExchange("exchange.name", configurationMock.Object);
        }
        
        [Fact]
        public void ShouldProperlyThrowExceptionWhenRegisteringConsumptionExchangeWhenUniversalExchangeWithSameNameAlreadyAdded()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    new ServiceCollection()
                        .AddExchange("exchange.name", new RabbitMqExchangeOptions(), ClientExchangeType.Universal)
                        .AddExchange("exchange.name", new RabbitMqExchangeOptions(), ClientExchangeType.Consumption);
                });
        }
        
        [Fact]
        public void ShouldProperlyThrowExceptionWhenRegisteringProductionExchangeWhenUniversalExchangeWithSameNameAlreadyAdded()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    new ServiceCollection()
                        .AddExchange("exchange.name", new RabbitMqExchangeOptions(), ClientExchangeType.Universal)
                        .AddExchange("exchange.name", new RabbitMqExchangeOptions(), ClientExchangeType.Production);
                });
        }
    }
}