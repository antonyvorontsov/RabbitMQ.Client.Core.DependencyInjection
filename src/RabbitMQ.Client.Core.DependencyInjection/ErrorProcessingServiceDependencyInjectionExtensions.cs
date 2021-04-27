using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// DI extensions for custom error processing service implementations.
    /// </summary>
    public static class ErrorProcessingServiceDependencyInjectionExtensions
    {
        /// <summary>
        /// Add custom error processing service.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <typeparam name="T">Type of a custom error processing service.</typeparam>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddCustomMessageHandlingErrorProcessingService<T>(this IServiceCollection services)
            where T : class, IErrorProcessingService
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IErrorProcessingService));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            return services.AddSingleton<IErrorProcessingService, T>();
        }
    }
}