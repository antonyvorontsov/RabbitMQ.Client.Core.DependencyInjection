using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Middlewares;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class BaseBatchMessageHandlerTests
    {
        private readonly TimeSpan _globalTestsTimeout = TimeSpan.FromSeconds(60);
        
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
            connectionFactoryMock.Setup(x => x.CreateRabbitMqConnection(It.IsAny<RabbitMqServiceOptions>()))
                .Returns(connectionMock.Object);

            var consumer = new AsyncEventingBasicConsumer(channelMock.Object);
            connectionFactoryMock.Setup(x => x.CreateConsumer(It.IsAny<IModel>()))
                .Returns(consumer);

            var callerMock = new Mock<IStubCaller>();

            using var messageHandler = CreateBatchMessageHandler(
                queueName,
                prefetchCount,
                null,
                connectionFactoryMock.Object,
                callerMock.Object,
                Enumerable.Empty<IBatchMessageHandlingMiddleware>());
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
            callerMock.Verify(x => x.Call(It.IsAny<ReadOnlyMemory<byte>>()), Times.Exactly(processedMessages));

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
            connectionFactoryMock.Setup(x => x.CreateRabbitMqConnection(It.IsAny<RabbitMqServiceOptions>()))
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

            using var messageHandler = CreateBatchMessageHandler(
                queueName,
                prefetchCount,
                handlingPeriod,
                connectionFactoryMock.Object,
                caller,
                Enumerable.Empty<IBatchMessageHandlingMiddleware>());
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
            }

            callerMock.Verify(x => x.EmptyCall(), Times.Exactly(numberOfSmallBatches));
            callerMock.Verify(x => x.Call(It.IsAny<ReadOnlyMemory<byte>>()), Times.Exactly(numberOfMessages));
            
            await messageHandler.StopAsync(CancellationToken.None);
        }
        
        [Fact]
        public async Task ShouldProperlyExecutePipeline()
        {
            const ushort prefetchCount = 5;
            const string queueName = "queue.name";

            var channelMock = new Mock<IModel>();
            var connectionMock = new Mock<IConnection>();
            connectionMock.Setup(x => x.CreateModel())
                .Returns(channelMock.Object);

            var connectionFactoryMock = new Mock<IRabbitMqConnectionFactory>();
            connectionFactoryMock.Setup(x => x.CreateRabbitMqConnection(It.IsAny<RabbitMqServiceOptions>()))
                .Returns(connectionMock.Object);

            var consumer = new AsyncEventingBasicConsumer(channelMock.Object);
            connectionFactoryMock.Setup(x => x.CreateConsumer(It.IsAny<IModel>()))
                .Returns(consumer);

            var callerMock = new Mock<IStubCaller>();

            var orderingMap = new Dictionary<int, int>();
            var firstMiddleware = new StubBatchMessageHandlingMiddleware(1, orderingMap);
            var secondMiddleware = new StubBatchMessageHandlingMiddleware(2, orderingMap);
            var thirdMiddleware = new StubBatchMessageHandlingMiddleware(3, orderingMap);
            
            var middlewares = new List<IBatchMessageHandlingMiddleware>
            {
                firstMiddleware,
                secondMiddleware,
                thirdMiddleware
            };

            using var messageHandler = CreateBatchMessageHandler(
                queueName,
                prefetchCount,
                null,
                connectionFactoryMock.Object,
                callerMock.Object,
                middlewares);
            await messageHandler.StartAsync(CancellationToken.None);

            for (var i = 0; i < prefetchCount; i++)
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

            callerMock.Verify(x => x.EmptyCall(), Times.Once);
            Assert.Equal(1, orderingMap[thirdMiddleware.Number]);
            Assert.Equal(2, orderingMap[secondMiddleware.Number]);
            Assert.Equal(3, orderingMap[firstMiddleware.Number]);
            
            await messageHandler.StopAsync(CancellationToken.None);
        }

        private static BaseBatchMessageHandler CreateBatchMessageHandler(
            string queueName,
            ushort prefetchCount,
            TimeSpan? handlingPeriod,
            IRabbitMqConnectionFactory connectionFactory,
            IStubCaller caller,
            IEnumerable<IBatchMessageHandlingMiddleware> middlewares)
        {
            var connectionOptions = new BatchConsumerConnectionOptions(typeof(StubBaseBatchMessageHandler), new RabbitMqServiceOptions());
            var loggingServiceMock = new Mock<ILoggingService>();
            return new StubBaseBatchMessageHandler(
                caller,
                connectionFactory,
                new List<BatchConsumerConnectionOptions> { connectionOptions },
                middlewares,
                loggingServiceMock.Object)
            {
                QueueName = queueName,
                PrefetchCount = prefetchCount,
                MessageHandlingPeriod = handlingPeriod
            };
        }
    }
}