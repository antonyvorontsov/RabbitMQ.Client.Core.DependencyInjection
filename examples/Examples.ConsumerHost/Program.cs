using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection;

namespace Examples.ConsumerHost
{
    public static class Program
    {
        public static async Task Main()
        {
            var builder = new HostBuilder()
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
              })
              .ConfigureLogging((hostingContext, logging) => {
                  logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                  logging.AddConsole();
              })
              .ConfigureServices((hostContext, services) =>
              {
                  var rabbitMqSection = hostContext.Configuration.GetSection("RabbitMq");
                  var exchangeSection = hostContext.Configuration.GetSection("RabbitMqExchange");

                  services.AddRabbitMqServices(rabbitMqSection)
                      .AddConsumptionExchange("exchange.name", exchangeSection)
                      .AddMessageHandlerTransient<CustomMessageHandler>("routing.key");
              });
            await builder.RunConsoleAsync();
        }
    }
}