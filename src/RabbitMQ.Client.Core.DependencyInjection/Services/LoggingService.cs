using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    /// <inheritdoc />
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly bool disableLogging;

        public LoggingService(
            ILogger<LoggingService> logger,
            IOptions<BehaviourConfiguration> options)
        {
            _logger = logger;
            disableLogging = options.Value.DisableInternalLogging;
        }

        /// <inheritdoc />
        public void LogError(Exception exception, string message)
        {
            // We do not disable logging for errors. Warning or debug messages are okay to skip, but skipping errors is risky.
            _logger.LogError(new EventId(), exception, message);
        }

        /// <inheritdoc />
        public void LogWarning(string message)
        {
            if (disableLogging)
            {
                return;
            }

            _logger.LogWarning(message);
        }

        /// <inheritdoc />
        public void LogInformation(string message)
        {
            if (disableLogging)
            {
                return;
            }

            _logger.LogInformation(message);
        }

        /// <inheritdoc />
        public void LogDebug(string message)
        {
            if (disableLogging)
            {
                return;
            }

            _logger.LogDebug(message);
        }
    }
}