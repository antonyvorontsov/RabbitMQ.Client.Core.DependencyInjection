using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Custom wrapper of default Microsoft logger.
    /// </summary>
    /// <remarks>
    /// This service is made for potential logging disabling when it is not necessary.
    /// </remarks>
    public interface ILoggingService
    {
        /// <summary>
        /// Log occured error.
        /// </summary>
        void LogError(Exception exception, string message);
        
        /// <summary>
        /// Log warning.
        /// </summary>
        void LogWarning(string message);

        /// <summary>
        /// Log information.
        /// </summary>
        void LogInformation(string message);

        /// <summary>
        /// Log debug.
        /// </summary>
        void LogDebug(string message);
    }
}