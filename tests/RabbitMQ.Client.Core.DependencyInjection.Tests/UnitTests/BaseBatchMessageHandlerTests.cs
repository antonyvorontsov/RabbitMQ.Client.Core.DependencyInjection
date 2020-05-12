using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.BatchMessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests.Stubs;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class BaseBatchMessageHandlerTests
    {
        [Theory]
        [InlineData(1, 10)]
        [InlineData(5, 47)]
        [InlineData(10, 185)]
        [InlineData(16, 200)]
        [InlineData(20, 310)]
        [InlineData(25, 400)]
        public async Task ShouldProperlyHandlerMessagesByBatches(ushort prefetchCount, int numberOfMessages)
        {
            const string queueName = "queue.name";

            var channelMock = new Mock<IModel>();
            var connectionMock = new Mock<IConnection>();
            connectionMock.Setup(x => x.CreateModel())
                .Returns(channelMock.Object);
            
            var connectionFactoryMock = new Mock<IRabbitMqConnectionFactory>();
            connectionFactoryMock.Setup(x => x.CreateRabbitMqConnection(It.IsAny<RabbitMqClientOptions>()))
                .Returns(connectionMock.Object);

            var consumer = new AsyncEventingBasicConsumer(channelMock.Object);
            connectionFactoryMock.Setup(x => x.CreateConsumer(It.IsAny<IModel>()))
                .Returns(consumer);
            
            var callerMock = new Mock<IStubCaller>();
            
            var messageHandler = CreateBatchMessageHandler(queueName, prefetchCount, connectionFactoryMock.Object, callerMock.Object);
            await messageHandler.StartAsync(CancellationToken.None);

            for (var i = 0; i < numberOfMessages; i++)
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

            var numberOfBatches = numberOfMessages / prefetchCount;
            callerMock.Verify(x => x.EmptyCall(), Times.Exactly(numberOfBatches));

            var processedMessages = numberOfBatches * prefetchCount;
            callerMock.Verify(x => x.Call(It.IsAny<ReadOnlyMemory<byte>>()), Times.Exactly(processedMessages));
            
            await messageHandler.StopAsync(CancellationToken.None);
        }

        static BaseBatchMessageHandler CreateBatchMessageHandler(
            string queueName,
            ushort prefetchCount,
            IRabbitMqConnectionFactory connectionFactory,
            IStubCaller caller)
        {
            var connectionOptions = new BatchConsumerConnectionOptions
            {
                Type = typeof(StubBaseBatchMessageHandler),
                ClientOptions = new RabbitMqClientOptions()
            };
            var loggerMock = new Mock<ILogger<StubBaseBatchMessageHandler>>();
            return new StubBaseBatchMessageHandler(
                caller,
                connectionFactory,
                new List<BatchConsumerConnectionOptions> { connectionOptions },
                loggerMock.Object)
            {
                QueueName = queueName,
                PrefetchCount = prefetchCount
            };
        }
    }
}