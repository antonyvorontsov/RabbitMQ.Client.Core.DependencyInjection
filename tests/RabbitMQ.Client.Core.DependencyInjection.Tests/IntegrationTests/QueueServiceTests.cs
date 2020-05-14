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
        
        [Fact]
        public async Task ShouldProperlyPublishAndConsumeMessages()
        {
            var clientOptions = new RabbitMqClientOptions
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            
            var exchangeOptions = new RabbitMqExchangeOptions
            {
                Type = "direct",
                DeadLetterExchange = "exchange.dlx",
                Queues = new List<RabbitMqQueueOptions>
                {
                    new RabbitMqQueueOptions
                    {
                        Name = "test.queue",
                        RoutingKeys = new HashSet<string> { "routing.key" }
                    }
                }
            };

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
                .AddRabbitMqClient(clientOptions)
                .AddConsumptionExchange("exchange.name", exchangeOptions)
                .AddMessageHandlerTransient<StubMessageHandler>("routing.key");
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var queueService = serviceProvider.GetRequiredService<IQueueService>();
            queueService.StartConsuming();
            
            var resetEvent = new AutoResetEvent(false);
            consumer.Received += async (sender, @event) =>
            {
                resetEvent.Set();
            };

            await queueService.SendAsync("message", "exchange.name", "routing.key");
            resetEvent.WaitOne(_globalTestsTimeout);
            callerMock.Verify(x => x.Call(It.IsAny<string>()), Times.Once);
        }
    }
}