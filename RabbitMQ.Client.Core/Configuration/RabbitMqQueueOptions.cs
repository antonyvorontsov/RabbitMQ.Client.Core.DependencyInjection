using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Configuration
{
    /// <summary>
    /// Модель (опций) очереди сообщений.
    /// </summary>
    public class RabbitMqQueueOptions
    {
        /// <summary>
        /// Наименование очереди.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Восстановить после перезапуска (хранить сообщения на диске).
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// Использовать только данным подключением.
        /// </summary>
        public bool Exclusive { get; set; } = false;

        /// <summary>
        /// Автоматически удалить, когда все подключенные получатели отключатся.
        /// </summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// Коллекция ключей маршрутизации, на которые смотрит очередь.
        /// </summary>
        public HashSet<string> RoutingKeys { get; set; } = new HashSet<string>();

        /// <summary>
        /// Дополнительные аргументы.
        /// TODO: на потом, а пока пустой словарь.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }
}