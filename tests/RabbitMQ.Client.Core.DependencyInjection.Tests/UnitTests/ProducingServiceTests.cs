using System;
using System.Collections.Generic;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class ProducingServiceTests
    {
        [Fact]
        public void ShouldProperlyThrowExceptionWhenThereAreNoProductionExchanges()
        {
            var consumptionExchange = new RabbitMqExchange("exchange", ClientExchangeType.Consumption, new RabbitMqExchangeOptions());
            var service = CreateService(new[] { consumptionExchange });
            Assert.Throws<ArgumentException>(() => service.ValidateArguments("another.exchange", "routing.key"));
        }

        [Fact]
        public void ShouldProperlyThrowExceptionWhenThereIsAnExchangeWithSameNameButWithConsumptionType()
        {
            const string exchangeName = "exchange";
            var consumptionExchange = new RabbitMqExchange(exchangeName, ClientExchangeType.Consumption, new RabbitMqExchangeOptions());
            var service = CreateService(new[] { consumptionExchange });
            Assert.Throws<ArgumentException>(() => service.ValidateArguments(exchangeName, "routing.key"));
        }

        [Fact]
        public void ShouldProperlyValidateWhenThereIsProductionExchange()
        {
            const string exchangeName = "exchange";
            var consumptionExchange = new RabbitMqExchange(exchangeName, ClientExchangeType.Production, new RabbitMqExchangeOptions());
            var service = CreateService(new[] { consumptionExchange });
            service.ValidateArguments(exchangeName, "routing.key");
        }

        [Fact]
        public void ShouldProperlyValidateWhenThereIsUiversalExchange()
        {
            const string exchangeName = "exchange";
            var consumptionExchange = new RabbitMqExchange(exchangeName, ClientExchangeType.Universal, new RabbitMqExchangeOptions());
            var service = CreateService(new[] { consumptionExchange });
            service.ValidateArguments(exchangeName, "routing.key");
        }

        private static ProducingService CreateService(IEnumerable<RabbitMqExchange> exchanges)
        {
            return new ProducingService(exchanges);
        }
    }
}