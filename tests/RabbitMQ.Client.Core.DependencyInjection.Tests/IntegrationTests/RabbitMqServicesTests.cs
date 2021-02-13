using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.IntegrationTests
{
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public class RabbitMqServicesTests
    {
        private readonly TimeSpan _globalTestsTimeout = TimeSpan.FromSeconds(60);

        private const string DefaultExchangeName = "exchange.name";
        private const string FirstRoutingKey = "first.routing.key";
        private const string SecondRoutingKey = "second.routing.key";
        private const int RequeueAttempts = 4;

        [Fact]
        public async Task ShouldProperlyPublishAndConsumeMessages()
        {
            var connectionFactoryMock = new Mock<RabbitMqConnectionFactory> { CallBase = true }
                .As<IRabbitMqConnectionFactory>();

            AsyncEventingBasicConsumer consumer = null;
            connectionFactoryMock.Setup(x => x.CreateConsumer(It.IsAny<IModel>()))
                .Returns<IModel>(channel =>
                {
                    consumer = new AsyncEventingBasicConsumer(channel);
                    return consumer;
                });

            var callerMock = new Mock<IStubCaller>();

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton(connectionFactoryMock.Object)
                .AddSingleton(callerMock.Object)
                .AddRabbitMqServices(GetClientOptions())
                .AddConsumptionExchange(DefaultExchangeName, GetExchangeOptions())
                .AddMessageHandlerTransient<StubMessageHandler>(FirstRoutingKey)
                .AddAsyncMessageHandlerTransient<StubAsyncMessageHandler>(SecondRoutingKey);

            await using var serviceProvider = serviceCollection.BuildServiceProvider();
            var consumingService = serviceProvider.GetRequiredService<IConsumingService>();
            var producingService = serviceProvider.GetRequiredService<IProducingService>();
            consumingService.StartConsuming();

            using var resetEvent = new AutoResetEvent(false);
            consumer.Received += (_, _) =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            };

            await producingService.SendAsync(new { Message = "message" }, DefaultExchangeName, FirstRoutingKey);
            resetEvent.WaitOne(_globalTestsTimeout);
            callerMock.Verify(x => x.Call(It.IsAny<string>()), Times.Exactly(2));

            await producingService.SendAsync(new { Message = "message" }, DefaultExchangeName, SecondRoutingKey);
            resetEvent.WaitOne(_globalTestsTimeout);
            callerMock.Verify(x => x.CallAsync(It.IsAny<string>()), Times.Exactly(2));
        }
        
        [Fact]
        public async Task ShouldProperlyRequeueMessages()
        {
            var connectionFactoryMock = new Mock<RabbitMqConnectionFactory> { CallBase = true }
                .As<IRabbitMqConnectionFactory>();

            AsyncEventingBasicConsumer consumer = null;
            connectionFactoryMock.Setup(x => x.CreateConsumer(It.IsAny<IModel>()))
                .Returns<IModel>(channel =>
                {
                    consumer = new AsyncEventingBasicConsumer(channel);
                    return consumer;
                });

            var callerMock = new Mock<IStubCaller>();

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton(connectionFactoryMock.Object)
                .AddSingleton(callerMock.Object)
                .AddRabbitMqServices(GetClientOptions())
                .AddConsumptionExchange(DefaultExchangeName, GetExchangeOptions())
                .AddMessageHandlerTransient<StubExceptionMessageHandler>(FirstRoutingKey);

            await using var serviceProvider = serviceCollection.BuildServiceProvider();
            var consumingService = serviceProvider.GetRequiredService<IConsumingService>();
            var producingService = serviceProvider.GetRequiredService<IProducingService>();
            
            consumingService.StartConsuming();

            using var resetEvent = new AutoResetEvent(false);
            consumer.Received += (_, _) =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            };

            await producingService.SendAsync(new { Message = "message" }, DefaultExchangeName, FirstRoutingKey);

            for (var i = 1; i <= RequeueAttempts + 1; i++)
            {
                resetEvent.WaitOne(_globalTestsTimeout);
            }
            callerMock.Verify(x => x.Call(It.IsAny<string>()), Times.Exactly(RequeueAttempts + 1));
        }

        private static RabbitMqServiceOptions GetClientOptions() =>
            new()
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

        private static RabbitMqExchangeOptions GetExchangeOptions() =>
            new()
            {
                Type = "direct",
                DeadLetterExchange = "exchange.dlx",
                RequeueAttempts = RequeueAttempts,
                RequeueTimeoutMilliseconds = 50,
                Queues = new List<RabbitMqQueueOptions>
                {
                    new()
                    {
                        Name = "test.queue",
                        RoutingKeys = new HashSet<string> { FirstRoutingKey, SecondRoutingKey }
                    }
                }
            };
    }
}