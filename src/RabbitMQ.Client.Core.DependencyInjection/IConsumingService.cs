namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// Custom RabbitMQ consuming service interface.
    /// </summary>
    public interface IConsumingService
    {
        /// <summary>
        /// RabbitMQ consuming connection.
        /// </summary>
        IConnection ConsumingConnection { get; }

        /// <summary>
        /// RabbitMQ consuming channel.
        /// </summary>
        IModel ConsumingChannel { get; }

        /// <summary>
        /// Start consuming (getting messages).
        /// </summary>
        void StartConsuming();
    }
}