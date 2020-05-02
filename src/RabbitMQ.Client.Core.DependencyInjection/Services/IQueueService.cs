namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <summary>
    /// Custom RabbitMQ queue service interface.
    /// </summary>
    public interface IQueueService : IConsumingService, IProducingService
    {
    }
}