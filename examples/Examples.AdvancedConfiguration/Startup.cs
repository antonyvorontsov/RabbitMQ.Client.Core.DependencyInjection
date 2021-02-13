using Examples.AdvancedConfiguration.MessageHandlers;
using Examples.AdvancedConfiguration.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;

namespace Examples.AdvancedConfiguration
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Configurations are the same (same user) and same password, but the only difference - names of connections.
            var rabbitMqConsumerSection = Configuration.GetSection("RabbitMqConsumer");
            var rabbitMqProducerSection = Configuration.GetSection("RabbitMqProducer");

            var producingExchangeSection = Configuration.GetSection("ProducingExchange");
            var consumingExchangeSection = Configuration.GetSection("ConsumingExchange");

            // There is an example of configuring different message handlers with different parameters.
            // You can set collection of routing keys or specify the exact exchange that will be listened by giver routing keys (or route patterns) by message handlers.
            // You can also register singleton or transient RabbitMQ clients (IConsumer and IProducer) and message handlers.
            // There are a lot of different extension methods that is better take a closer look to.
            services.AddRabbitMqConsumer(rabbitMqConsumerSection)
                .AddRabbitMqProducer(rabbitMqProducerSection)
                .AddProductionExchange("exchange.to.send.messages.only", producingExchangeSection)
                .AddConsumptionExchange("consumption.exchange", consumingExchangeSection)
                .AddMessageHandlerTransient<CustomMessageHandler>("routing.key")
                .AddAsyncMessageHandlerTransient<CustomAsyncMessageHandler>(new[] { "routing.key", "another.routing.key" });

            services.AddHostedService<ConsumingHostedService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}