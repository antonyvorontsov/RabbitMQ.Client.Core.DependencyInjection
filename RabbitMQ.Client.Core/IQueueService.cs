using System;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Интерфейс кастомного клиента обмена сообщениями RabbitMQ.
    /// </summary>
    public interface IQueueService : IDisposable
    {
        /// <summary>
        /// Интерфейс соединения RabbitMQ.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Канал RabbitMQ.
        /// </summary>
        IModel Channel { get; }
        
        /// <summary>
        /// Начать "прослушивать" очереди (получать сообщения).
        /// </summary>
        void StartConsuming();

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <typeparam name="T">Класс.</typeparam>
        /// <param name="object">Объект, отправляемый в качестве сообщения.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        void Send<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="json">Сообщение в формате json.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        void SendJson(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="bytes">Собщение в формате массива байт.</param>
        /// <param name="properties"></param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey);

        /// <summary>
        /// Асинхронно отправить сообщение.
        /// </summary>
        /// <typeparam name="T">Класс.</typeparam>
        /// <param name="object">Объект, отправляемый в качестве сообщения.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        /// <returns></returns>
        Task SendAsync<T>(T @object, string exchangeName, string routingKey) where T : class;

        /// <summary>
        /// Асинхронно отправить сообщение.
        /// </summary>
        /// <param name="json">Сообщение в формате json.</param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        /// <returns></returns>
        Task SendJsonAsync(string json, string exchangeName, string routingKey);

        /// <summary>
        /// Асинхронно отправить сообщение.
        /// </summary>
        /// <param name="bytes">Собщение в формате массива байт.</param>
        /// <param name="properties"></param>
        /// <param name="exchangeName">Наименование обменника.</param>
        /// <param name="routingKey">Ключ маршрутизации.</param>
        /// <returns></returns>
        Task SendAsync(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey);
    }
}