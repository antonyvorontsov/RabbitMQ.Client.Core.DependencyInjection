using System.Collections.Generic;
using Examples.ManualAck.MessageHandlers;
using Examples.ManualAck.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace Examples.ManualAck
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
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
                    new()
                    {
                        Name = "myqueue",
                        RoutingKeys = new HashSet<string> { "routing.key" }
                    }
                },
                DisableAutoAck = true
            };

            services.AddRabbitMqServices(rabbitMqConfiguration)
                .AddExchange("exchange", exchangeOptions)
                .AddMessageHandlerTransient<CustomMessageHandler>("routing.key")
                .AddMessageHandlingMiddleware<CustomMessageHandlingMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}