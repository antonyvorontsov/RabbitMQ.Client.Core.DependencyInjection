using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
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
            var consumingService = CreateConsumingService(messageHandlingPipelineExecutingServiceMock.Object);
            
            consumingService.UseConnection(connectionMock.Object);
            consumingService.UseChannel(channelMock.Object);
            consumingService.UseConsumer(consumer);

            await consumer.HandleBasicDeliver(
                "1",
                0,
                false,
                "exchange",
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

            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<Action<BasicDeliverEventArgs>>()), Times.Exactly(numberOfMessages));
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
            var consumingService = CreateConsumingService(messageHandlingPipelineExecutingServiceMock.Object);
            
            consumingService.UseConnection(connectionMock.Object);
            consumingService.UseChannel(channelMock.Object);
            consumingService.UseConsumer(consumer);
            
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

            messageHandlingPipelineExecutingServiceMock.Verify(x => x.Execute(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<Action<BasicDeliverEventArgs>>()), Times.Exactly(numberOfMessages));

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

        private static IConsumingService CreateConsumingService(IMessageHandlingPipelineExecutingService messageHandlingPipelineExecutingService) =>
            new ConsumingService(messageHandlingPipelineExecutingService, new List<RabbitMqExchange>());
    }
}