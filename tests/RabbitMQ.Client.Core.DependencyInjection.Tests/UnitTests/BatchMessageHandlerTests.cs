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
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class BatchMessageHandlerTests
    {
        readonly TimeSpan _globalTestsTimeout = TimeSpan.FromSeconds(60);
        
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

            using var messageHandler = CreateBatchMessageHandler(queueName, prefetchCount, null, connectionFactoryMock.Object, callerMock.Object);
            await messageHandler.StartAsync(CancellationToken.None);

            for (var i = 0; i < numberOfMessages; i++)
            {
                await consumer.HandleBasicDeliver(
                    "1",
                    (ulong)i,
                    false,
                    "exchange",
                    "routing,key",
                    null,
                    new ReadOnlyMemory<byte>());
            }

            var numberOfBatches = numberOfMessages / prefetchCount;
            callerMock.Verify(x => x.EmptyCall(), Times.Exactly(numberOfBatches));

            var processedMessages = numberOfBatches * prefetchCount;
            callerMock.Verify(x => x.Call(It.IsAny<string>()), Times.Exactly(processedMessages));

            await messageHandler.StopAsync(CancellationToken.None);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(16)]
        [InlineData(40)]
        [InlineData(57)]
        public async Task ShouldProperlyHandlerMessagesByTimer(int numberOfMessages)
        {
            const string queueName = "queue.name";
            const ushort prefetchCount = 10;
            var handlingPeriod = TimeSpan.FromMilliseconds(100);

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

            using var waitHandle = new AutoResetEvent(false);
            var callerMock = new Mock<IStubCaller>();
            var caller = new StubCallerDecorator(callerMock.Object)
            {
                WaitHandle = waitHandle
            };

            using var messageHandler = CreateBatchMessageHandler(queueName, prefetchCount, handlingPeriod, connectionFactoryMock.Object, caller);
            await messageHandler.StartAsync(CancellationToken.None);

            const int smallBatchSize = prefetchCount - 1;
            var numberOfSmallBatches = (int)Math.Ceiling((double)numberOfMessages / smallBatchSize);
            for (var b = 0; b < numberOfSmallBatches; b++)
            {
                var lowerBound = b * smallBatchSize;
                var upperBound = (b + 1) * smallBatchSize > numberOfMessages ? numberOfMessages : (b + 1) * smallBatchSize;
                for (var i = lowerBound; i < upperBound; i++)
                {
                    await consumer.HandleBasicDeliver(
                        "1",
                        (ulong)i,
                        false,
                        "exchange",
                        "routing,key",
                        null,
                        new ReadOnlyMemory<byte>());
                }
                
                waitHandle.WaitOne(_globalTestsTimeout);
                callerMock.Verify(x => x.EmptyCall(), Times.Exactly(b + 1));
                callerMock.Verify(x => x.Call(It.IsAny<string>()), Times.Exactly(upperBound));
            }

            await messageHandler.StopAsync(CancellationToken.None);
        }

        static BatchMessageHandler CreateBatchMessageHandler(
            string queueName,
            ushort prefetchCount,
            TimeSpan? handlingPeriod,
            IRabbitMqConnectionFactory connectionFactory,
            IStubCaller caller)
        {
            var connectionOptions = new BatchConsumerConnectionOptions
            {
                Type = typeof(StubBatchMessageHandler),
                ClientOptions = new RabbitMqClientOptions()
            };
            var loggerMock = new Mock<ILogger<StubBatchMessageHandler>>();
            return new StubBatchMessageHandler(
                caller,
                connectionFactory,
                new List<BatchConsumerConnectionOptions> { connectionOptions },
                loggerMock.Object)
            {
                QueueName = queueName,
                PrefetchCount = prefetchCount,
                MessageHandlingPeriod = handlingPeriod
            };
        }
    }
}