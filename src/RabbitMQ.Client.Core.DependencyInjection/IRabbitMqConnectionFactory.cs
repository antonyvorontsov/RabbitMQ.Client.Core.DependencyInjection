using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Interface of the service that contains business logic of creating RabbitMQ connections depending on options <see cref="RabbitMqClientOptions"/>.
    /// </summary>
    public interface IRabbitMqConnectionFactory
    {
        /// <summary>
        /// Create a RabbitMQ connection.
        /// </summary>
        /// <returns>An instance of connection <see cref="IConnection"/>.</returns>
        IConnection CreateRabbitMqConnection();
    }
}