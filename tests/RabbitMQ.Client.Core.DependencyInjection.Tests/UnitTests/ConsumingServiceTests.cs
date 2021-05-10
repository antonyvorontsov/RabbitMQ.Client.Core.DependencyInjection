using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class ConsumingServiceTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(20)]
        [InlineData(25)]
        public async Task ShouldProperlyConsumeMessages(int numberOfMessages)
        {
            var channelMock = new Mock<IModel>();
            var connectionMock = new Mock<IConnection>();
            var consumer = new AsyncEventingBasicConsumer(channelMock.Object);

            var messageHandlingPipelineExecutingServiceMock = new Mock<IMessageHandlingPipelineExecutingService>();
            const string exchangeName = "exchange";
            var exchange = new RabbitMqExchange(exchangeName, ClientExchangeType.Consumption, new RabbitMqExchangeOptions());
            var consumingService = CreateConsumingService(messageHandlingPipelineExecutingServiceMock.Object, new[] { exchange });

            var declaration = (IConsumingServiceDeclaration)consumingService;
            declaration.UseConnection(connectionMock.Object);
            declaration.UseChannel(channelMock.Object);
            declaration.UseConsumer(consumer);

            await consumer.HandleBasicDeliver(
                "1",
                0,
                false,
                exchangeName,
                "routing,key",
                null,
                new ReadOnlyMemory<byte>());
            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<Action<BasicDeliverEventArgs>>()), Times.Never);

            consumingService.StartConsuming();

            for (var i = 1; i <= numberOfMessages; i++)
            {
                await consumer.HandleBasicDeliver(
                    "1",
                    (ulong)numberOfMessages,
                    false,
                    "exchange",
                    "routing,key",
                    null,
                    new ReadOnlyMemory<byte>());
            }

            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), It.IsNotNull<Action<BasicDeliverEventArgs>>()), Times.Exactly(numberOfMessages));
        }


        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(20)]
        [InlineData(25)]
        public async Task ShouldProperlyConsumeMessagesButWithoutAutoAck(int numberOfMessages)
        {
            var channelMock = new Mock<IModel>();
            var connectionMock = new Mock<IConnection>();
            var consumer = new AsyncEventingBasicConsumer(channelMock.Object);

            var messageHandlingPipelineExecutingServiceMock = new Mock<IMessageHandlingPipelineExecutingService>();

            const string exchangeName = "exchange";
            var exchange = new RabbitMqExchange(exchangeName, ClientExchangeType.Consumption, new RabbitMqExchangeOptions { DisableAutoAck = true });
            var consumingService = CreateConsumingService(messageHandlingPipelineExecutingServiceMock.Object, new[] { exchange });

            var declaration = (IConsumingServiceDeclaration)consumingService;
            declaration.UseConnection(connectionMock.Object);
            declaration.UseChannel(channelMock.Object);
            declaration.UseConsumer(consumer);

            await consumer.HandleBasicDeliver(
                "1",
                0,
                false,
                exchangeName,
                "routing,key",
                null,
                new ReadOnlyMemory<byte>());
            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<Action<BasicDeliverEventArgs>>()), Times.Never);

            consumingService.StartConsuming();

            for (var i = 1; i <= numberOfMessages; i++)
            {
                await consumer.HandleBasicDeliver(
                    "1",
                    (ulong)numberOfMessages,
                    false,
                    "exchange",
                    "routing,key",
                    null,
                    new ReadOnlyMemory<byte>());
            }

            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), null), Times.Exactly(numberOfMessages));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(20)]
        [InlineData(25)]
        public async Task ShouldProperlyStopConsumingMessages(int numberOfMessages)
        {
            var channelMock = new Mock<IModel>();
            var connectionMock = new Mock<IConnection>();
            var consumer = new AsyncEventingBasicConsumer(channelMock.Object);

            var messageHandlingPipelineExecutingServiceMock = new Mock<IMessageHandlingPipelineExecutingService>();
            const string exchangeName = "exchange";
            var exchange = new RabbitMqExchange(exchangeName, ClientExchangeType.Consumption, new RabbitMqExchangeOptions());
            var consumingService = CreateConsumingService(messageHandlingPipelineExecutingServiceMock.Object, new[] { exchange });

            var declaration = (IConsumingServiceDeclaration)consumingService;
            declaration.UseConnection(connectionMock.Object);
            declaration.UseChannel(channelMock.Object);
            declaration.UseConsumer(consumer);

            consumingService.StartConsuming();
            for (var i = 1; i <= numberOfMessages; i++)
            {
                await consumer.HandleBasicDeliver(
                    "1",
                    (ulong)numberOfMessages,
                    false,
                    exchangeName,
                    "routing,key",
                    null,
                    new ReadOnlyMemory<byte>());
            }

            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), It.IsNotNull<Action<BasicDeliverEventArgs>>()), Times.Exactly(numberOfMessages));

            consumingService.StopConsuming();
            await consumer.HandleBasicDeliver(
                "1",
                0,
                false,
                "exchange",
                "routing,key",
                null,
                new ReadOnlyMemory<byte>());

            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<Action<BasicDeliverEventArgs>>()), Times.Exactly(numberOfMessages));
        }

        private static IConsumingService CreateConsumingService(
            IMessageHandlingPipelineExecutingService messageHandlingPipelineExecutingService,
            IEnumerable<RabbitMqExchange> exchanges) =>
            new ConsumingService(messageHandlingPipelineExecutingService, exchanges);
    }
}