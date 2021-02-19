namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Custom RabbitMQ service interface.
    /// </summary>
    public interface IRabbitMqService
    {
        /// <summary>
        /// Specify the connection that will be used by the service.
        /// </summary>
        /// <param name="connection">Connection.</param>
        void UseConnection(IConnection connection);

        /// <summary>
        /// Specify the channel that will be used by the service.
        /// </summary>
        /// <param name="channel">channel.</param>
        void UseChannel(IModel channel);
    }
}