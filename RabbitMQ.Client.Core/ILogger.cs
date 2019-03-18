using Microsoft.Extensions.Logging;

namespace RabbitMQ.Client.Core
{
    /// <summary>
    /// Custom logger interface.
    /// </summary>
    /// <remarks>
    /// Allows handle logging by client logic e.g. writing logs in the database.
    /// </remarks>
    public interface ILogger
    {
        /// <summary>
        /// Write a log message.
        /// </summary>
        /// <param name="logLevel">Log message level.</param>
        /// <param name="message">Message.</param>
        void Log(Log​Level logLevel, string message);
    }
}