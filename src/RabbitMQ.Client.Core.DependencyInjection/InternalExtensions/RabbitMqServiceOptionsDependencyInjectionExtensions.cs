using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions
{
    /// <summary>
    /// Base DI extensions for RabbitMQ configuration options.
    /// </summary>
    internal static class RabbitMqServiceOptionsDependencyInjectionExtensions
    {
        internal static RabbitMqServiceOptions GetRabbitMqServiceOptionsInstance(IConfiguration configuration)
        {
            var options = new RabbitMqServiceOptions();
            configuration.Bind(options);
            return options;
        }

        internal static IServiceCollection ConfigureRabbitMqProducingOnlyServiceOptions(this IServiceCollection services, RabbitMqServiceOptions options) =>
            services.Configure<RabbitMqConnectionOptions>(x => x.ProducerOptions = options);

        internal static IServiceCollection ConfigureRabbitMqConnectionOptions(this IServiceCollection services, RabbitMqServiceOptions options) =>
            services.Configure<RabbitMqConnectionOptions>(x => { x.ProducerOptions = options; x.ConsumerOptions = options; });
    }
}