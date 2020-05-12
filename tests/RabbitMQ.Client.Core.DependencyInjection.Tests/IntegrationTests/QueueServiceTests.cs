using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.IntegrationTests
{
    public class QueueServiceTests
    {
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
            var serviceCollection = new ServiceCollection();

            var callerMock = new Mock<IStubCaller>();
            serviceCollection.AddRabbitMqClient(clientOptions)
                .AddConsumptionExchange("exchange.name", exchangeOptions)
                .AddMessageHandlerTransient<StubMessageHandler>("routing.key")
                .AddSingleton(callerMock.Object);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var queueService = serviceProvider.GetRequiredService<IQueueService>();
            queueService.StartConsuming();

            await queueService.SendAsync("message", "exchange.name", "routing.key");
        }
    }
}