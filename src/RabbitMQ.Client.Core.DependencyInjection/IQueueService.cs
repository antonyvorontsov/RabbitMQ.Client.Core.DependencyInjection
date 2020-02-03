namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Custom RabbitMQ queue service interface.
    /// </summary>
    public interface IQueueService : IConsumingService, IProducingService
    {
    }
}