using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace Examples.SslProducer
{
    public static class Program
    {
        private static IConfiguration _configuration;

        public static async Task Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var queueService = serviceProvider.GetRequiredService<IProducingService>();

            for (var i = 0; i < 10; i++)
            {
                var message = new
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
            // You can either use a json configuration or bind options by yourself.
            var rabbitMqConfiguration = _configuration.GetSection("RabbitMq");
            // There are both examples of json and manual configuration.
            //var rabbitMqConfiguration = GetClientOptions();

            var exchangeOptions = GetExchangeOptions();

            services.AddRabbitMqProducer(rabbitMqConfiguration)
                .AddProductionExchange("exchange.name", exchangeOptions);
        }

        private static RabbitMqServiceOptions GetClientOptions() =>
            new RabbitMqServiceOptions
            {
                UserName = "guest",
                Password = "guest",
                TcpEndpoints = new List<RabbitMqTcpEndpoint>
                {
                    new RabbitMqTcpEndpoint
                    {
                        HostName = "127.0.0.1",
                        Port = 5671,
                        SslOption = new RabbitMqSslOption
                        {
                            Enabled = true,
                            ServerName = "yourCA",
                            CertificatePath = "/path/tp/client-key-store.p12",
                            CertificatePassphrase = "yourPathPhrase",
                            AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch
                        }
                    }
                }
            };

        private static RabbitMqExchangeOptions GetExchangeOptions() =>
            new RabbitMqExchangeOptions
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
    }
}