using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Configuration
{
    /// <summary>
    /// Опции (конфигурация) обменника сообщений.
    /// </summary>
    public class RabbitMqExchangeOptions
    {
        /// <summary>
        /// Тип обменника.
        /// </summary>
        public string Type { get; set; } = "direct";

        /// <summary>
        /// Опция хранения сообщений.
        /// true - на диске,
        /// false - в кэше.
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// Автоматически удалять обменник, когда сообщения в нём заканчиваются.
        /// </summary>
        public bool AutoDelete { get; set; } = false;
        
        /// <summary>
        /// Дополнительные аргументы.
        /// TODO: на потом, а пока пустой словарь.
        /// </summary>
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Коллекция очередей, подписанных на точку обмена.
        /// </summary>
        public IList<RabbitMqQueueOptions> Queues { get; set; } = new List<RabbitMqQueueOptions>();    }
}