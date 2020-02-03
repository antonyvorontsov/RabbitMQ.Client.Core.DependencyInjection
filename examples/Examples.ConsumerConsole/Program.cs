using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using System.IO;

namespace Examples.ConsumerConsole
{
    public static class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var consumingService = serviceProvider.GetRequiredService<IConsumingService>();
            consumingService.StartConsuming();
            
            var producingService = serviceProvider.GetRequiredService<IProducingService>();
            
            // This is just an example.
            //producingService.SendString("I am sending messages!", "exchange", "routing.key");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            var rabbitMqSection = Configuration.GetSection("RabbitMq");
            var exchangeSection = Configuration.GetSection("RabbitMqExchange");

            services.AddRabbitMqConsumingClientSingleton(rabbitMqSection)
                .AddRabbitMqProducingClientSingleton(rabbitMqSection)
                .AddConsumptionExchange("exchange.name", exchangeSection)
                .AddAsyncMessageHandlerSingleton<CustomAsyncMessageHandler>("routing.key");
                //.AddAsyncNonCyclicMessageHandlerSingleton<CustomAsyncNonCyclicMessageHandler>("routing.key");
        }
    }
}