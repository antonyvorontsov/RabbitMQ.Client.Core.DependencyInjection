using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// ConsumingService declaration interface.
    /// </summary>
    internal interface IConsumingServiceDeclaration : IRabbitMqService
    {
        /// <summary>
        /// Specify a consumer instance that will be used by the service.
        /// </summary>
        /// <param name="consumer"></param>
        void UseConsumer(AsyncEventingBasicConsumer consumer);
    }
}