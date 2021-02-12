using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <summary>
    /// Hosted service that is responsible for starting the consumer.
    /// </summary>
    public class ConsumingHostedService : IHostedService
    {
        private readonly IConsumingService _consumingService;
        
        public ConsumingHostedService(IConsumingService consumingService)
        {
            _consumingService = consumingService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumingService.StartConsuming();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}