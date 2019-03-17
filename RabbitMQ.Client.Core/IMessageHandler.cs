using System.Collections.Generic;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Интерфейс обработчика полученных сообщений.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Коллекция ключей маршрутизации, которые прослушивает обработчик сообщений.
        /// </summary>
        IEnumerable<string> RoutingKeys { get; set; }

        /// <summary>
        /// Обработать сообщение.
        /// </summary>
        /// <param name="message">Входящее сообщение в формате json.</param>
        /// <param name="routingKey">Ключ маршрутизации, по которому сообщение поступило на обработку.</param>
        void Handle(string message, string routingKey);
    }
}