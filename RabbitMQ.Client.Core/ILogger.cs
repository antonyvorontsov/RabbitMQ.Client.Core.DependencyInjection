using Microsoft.Extensions.Logging;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Интерфейс клиентского логгера.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Записать лог.
        /// </summary>
        /// <param name="logLevel">Уровень сообщения.</param>
        /// <param name="message">Сообщение.</param>
        void Log(Log​Level logLevel, string message);
    }
}