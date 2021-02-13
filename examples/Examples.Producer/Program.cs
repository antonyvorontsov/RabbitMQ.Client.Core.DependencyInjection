using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Examples.Producer
{
    public static class Program
    {
        public static async Task Main()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var queueService = serviceProvider.GetRequiredService<IQueueService>();

            for (var i = 0; i < 10; i++)
            {
                var message = new Message
                {
                    Name = "Custom message",
                    Flag = true,
                    Index = i,
                    Numbers = new[] { 1, 2, 3 }
                };
                await queueService.SendAsync(message, "exchange.name", "routing.key");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var rabbitMqConfiguration = new RabbitMqServiceOptions
            {
                HostName = "127.0.0.1",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            var exchangeOptions = new RabbitMqExchangeOptions
            {
                Queues = new List<RabbitMqQueueOptions>
                {
                    new RabbitMqQueueOptions
                    {
                        Name = "myqueue",
                        RoutingKeys = new HashSet<string> { "routing.key" }
                    }
                }
            };
            services.AddRabbitMqClient(rabbitMqConfiguration)
                .AddProductionExchange("exchange.name", exchangeOptions);
        }
    }
}