using System;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Custom RabbitMQ queue service interface.
    /// </summary>
    public interface IQueueService : IDisposable
    {
        /// <summary>
        /// RabbitMQ connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// RabbitMQ channel.
        /// </summary>
        IModel Channel { get; }
        
        /// <summary>
        /// Start comsuming (getting messages).
        /// </summary>
        void StartConsuming();

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void SendJson(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        Task SendJsonAsync(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <returns></returns>
        Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey);
    }
}