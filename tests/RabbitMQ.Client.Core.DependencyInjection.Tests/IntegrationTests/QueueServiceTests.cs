using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.IntegrationTests
{
    public class QueueServiceTests
    {
        readonly TimeSpan _globalTestsTimeout = TimeSpan.FromSeconds(60);

        const string DefaultExchangeName = "exchange.name";
        const string FirstRoutingKey = "first.routing.key";
        const string SecondRoutingKey = "second.routing.key";
        
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
                .AddRabbitMqClient(GetClientOptions())
                .AddConsumptionExchange(DefaultExchangeName, GetExchangeOptions())
                .AddMessageHandlerTransient<StubMessageHandler>(FirstRoutingKey)
                .AddNonCyclicMessageHandlerTransient<StubNonCyclicMessageHandler>(FirstRoutingKey)
                .AddAsyncMessageHandlerTransient<StubAsyncMessageHandler>(SecondRoutingKey)
                .AddAsyncNonCyclicMessageHandlerTransient<StubAsyncNonCyclicMessageHandler>(SecondRoutingKey);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var queueService = serviceProvider.GetRequiredService<IQueueService>();
            queueService.StartConsuming();
            
            var resetEvent = new AutoResetEvent(false);
            consumer.Received += (sender, @event) =>
            {
                resetEvent.Set();
                return Task.CompletedTask;
            };

            await queueService.SendAsync(new { Message = "message" }, DefaultExchangeName, FirstRoutingKey);
            resetEvent.WaitOne(_globalTestsTimeout);
            callerMock.Verify(x => x.Call(It.IsAny<string>()), Times.Exactly(2));
            
            await queueService.SendAsync(new { Message = "message" }, DefaultExchangeName, SecondRoutingKey);
            resetEvent.WaitOne(_globalTestsTimeout);
            callerMock.Verify(x => x.CallAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        static RabbitMqClientOptions GetClientOptions() =>
            new RabbitMqClientOptions
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

        static RabbitMqExchangeOptions GetExchangeOptions() =>
            new RabbitMqExchangeOptions
            {
                Type = "direct",
                DeadLetterExchange = "exchange.dlx",
                Queues = new List<RabbitMqQueueOptions>
                {
                    new RabbitMqQueueOptions
                    {
                        Name = "test.queue",
                        RoutingKeys = new HashSet<string> { FirstRoutingKey, SecondRoutingKey }
                    }
                }
            };
    }
}