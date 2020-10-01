using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace Examples.BatchMessageHandler
{
    public static class Program
    {
        public static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Let's configure two different BatchMessageHandlers with different methods.
                    // First - configuring an appsettings.json section.
                    services.AddBatchMessageHandler<CustomBatchMessageHandler>(hostContext.Configuration.GetSection("RabbitMq"));
                    
                    // Second one - passing configuration instance.
                    var rabbitMqConfiguration = new RabbitMqClientOptions
                    {
                        HostName = "127.0.0.1",
                        Port = 5672,
                        UserName = "guest",
                        Password = "guest"
                    };
                    services.AddBatchMessageHandler<CustomBatchMessageHandler>(rabbitMqConfiguration);
                    
                    // Use either of them. Do not register batch message handlers multiple times in your real project.
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                });
            await builder.RunConsoleAsync();
        }
    }
}