using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Custom RabbitMQ consuming service interface.
    /// </summary>
    public interface IConsumingService
    {
        /// <summary>
        /// RabbitMQ consuming connection.
        /// </summary>
        IConnection? Connection { get; }

        /// <summary>
        /// RabbitMQ consuming channel.
        /// </summary>
        IModel? Channel { get; }

        /// <summary>
        /// Asynchronous consumer.
        /// </summary>
        AsyncEventingBasicConsumer? Consumer { get; }

        /// <summary>
        /// Start consuming (getting messages).
        /// </summary>
        void StartConsuming();

        /// <summary>
        /// Stop consuming (getting messages).
        /// </summary>
        void StopConsuming();
    }
}