using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    // TODO: change summary.
    /// <summary>
    /// Hosted service that is responsible for creating connections and channels for both producing and consuming services.
    /// </summary>
    public class ChannelDeclarationHostedService : IHostedService
    {
        private readonly IChannelDeclarationService _channelDeclarationService;

        public ChannelDeclarationHostedService(IChannelDeclarationService channelDeclarationService)
        {
            _channelDeclarationService = channelDeclarationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channelDeclarationService.SetConnectionInfrastructureForRabbitMqServices();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}