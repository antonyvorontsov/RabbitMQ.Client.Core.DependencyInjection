using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.ConsumerHost
{
    public class ConsumingService : IHostedService
    {
        readonly IQueueService _queueService;
        readonly ILogger<ConsumingService> _logger;

        public ConsumingService(
            IQueueService queueService,
            ILogger<ConsumingService> logger)
        {
            _queueService = queueService;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting consuming.");
            _queueService.StartConsuming();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping consuming.");
            return Task.CompletedTask;
        }
    }
}