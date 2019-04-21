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

            var queueService = serviceProvider.GetRequiredService<IQueueService>();
            queueService.StartConsuming();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            var rabbitMqSection = Configuration.GetSection("RabbitMq");
            var exchangeSection = Configuration.GetSection("RabbitMqExchange");

            services.AddRabbitMqClient(rabbitMqSection)
                .AddExchange("exchange.name", exchangeSection)
                .AddAsyncMessageHandlerSingleton<CustomAsyncMessageHandler>("routing.key");
        }
    }
}