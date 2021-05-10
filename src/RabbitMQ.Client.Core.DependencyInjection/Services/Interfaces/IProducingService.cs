using System;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Custom RabbitMQ producing service interface.
    /// </summary>
    public interface IProducingService
    {
        /// <summary>
        /// RabbitMQ producing connection.
        /// </summary>
        IConnection? Connection { get; }

        /// <summary>
        /// RabbitMQ producing channel.
        /// </summary>
        IModel? Channel { get; }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        void Send<T>(T @object, string exchangeName, string routingKey, int millisecondsDelay) where T : class;

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void SendJson(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        void SendJson(string json, string exchangeName, string routingKey, int millisecondsDelay);

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void SendString(string message, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        void SendString(string message, string exchangeName, string routingKey, int millisecondsDelay);

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        void Send(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey, int millisecondsDelay);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <typeparam name="T">Model class.</typeparam>
        /// <param name="object">Object message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey, int millisecondsDelay) where T : class;

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendJsonAsync(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <param name="json">Json message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        Task SendJsonAsync(string json, string exchangeName, string routingKey, int millisecondsDelay);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendStringAsync(string message, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        Task SendStringAsync(string message, string exchangeName, string routingKey, int millisecondsDelay);

        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        Task SendAsync(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey);

        /// <summary>
        /// Send a delayed message asynchronously.
        /// </summary>
        /// <param name="bytes">Byte array message.</param>
        /// <param name="properties">Message properties.</param>
        /// <param name="exchangeName">Exchange name.</param>
        /// <param name="routingKey">Routing key.</param>
        /// <param name="millisecondsDelay">Delay time in milliseconds.</param>
        Task SendAsync(ReadOnlyMemory<byte> bytes, IBasicProperties properties, string exchangeName, string routingKey, int millisecondsDelay);
    }
}