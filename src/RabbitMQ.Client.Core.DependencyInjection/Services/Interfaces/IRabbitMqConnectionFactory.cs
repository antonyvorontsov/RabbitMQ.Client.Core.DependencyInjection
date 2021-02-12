using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Interface of the service that is responsible for creating RabbitMQ connections depending on options <see cref="RabbitMqClientOptions"/>.
    /// </summary>
    public interface IRabbitMqConnectionFactory
    {
        /// <summary>
        /// Create a RabbitMQ connection.
        /// </summary>
        /// <param name="options">An instance of options <see cref="RabbitMqClientOptions"/>.</param>
        /// <returns>An instance of connection <see cref="IConnection"/>.</returns>
        /// <remarks>If options parameter is null the method return null too.</remarks>
        IConnection CreateRabbitMqConnection(RabbitMqClientOptions options);

        /// <summary>
        /// Create a consumer depending on the connection channel.
        /// </summary>
        /// <param name="channel">Connection channel.</param>
        /// <returns>A consumer instance <see cref="AsyncEventingBasicConsumer"/>.</returns>
        AsyncEventingBasicConsumer CreateConsumer(IModel channel);
    }
}