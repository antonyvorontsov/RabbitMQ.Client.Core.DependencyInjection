using RabbitMQ.Client.Core.Configuration;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Модель точки обмена (exchange).
    /// </summary>
    public class RabbitMqExchange
    {
        /// <summary>
        /// Наименование точки обмена.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Опции точки обмена.
        /// </summary>
        public RabbitMqExchangeOptions Options { get; set; }
    }
}