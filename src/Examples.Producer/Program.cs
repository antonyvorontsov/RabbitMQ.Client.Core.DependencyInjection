using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace Examples.Producer
{
    public static class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static async Task Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var queueService = serviceProvider.GetRequiredService<IQueueService>();

            var message = new Message
            {
                Name = "Custom message",
                Flag = true,
                Numbers = new[] { 1, 2, 3 }
            };
            await queueService.SendAsync(message, "exchange.name", "routing.key");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            var rabbitMqSection = Configuration.GetSection("RabbitMq");
            var exchangeSection = Configuration.GetSection("RabbitMqExchange");

            services.AddRabbitMqClient(rabbitMqSection)
                .AddExchange("exchange.name", exchangeSection);
        }
    }
}