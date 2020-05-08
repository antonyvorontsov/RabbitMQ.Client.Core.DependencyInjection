using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
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
    }
}