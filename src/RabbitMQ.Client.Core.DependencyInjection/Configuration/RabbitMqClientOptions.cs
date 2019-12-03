using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// RabbitMQ configuration model.
    /// </summary>
    public class RabbitMqClientOptions
    {
        /// <summary>
        /// RabbitMQ server.
        /// </summary>
        public string HostName { get; set; } = "127.0.0.1";

        /// <summary>
        /// Collection of RabbitMQ server host names.
        /// </summary>
        /// <remarks>
        /// It can be used when RabbitMQ HA cluster is set up and you have to connect multiple hosts.
        /// If HostNames collection is null or empty then HostName will be used to create connection.
        /// Otherwise HostNames collection will be used and HostName property value will be ignored.
        /// </remarks>
        public IEnumerable<string> HostNames { get; set; }

        /// <summary>
        /// Port.
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// UserName that connects to the server.
        /// </summary>
        public string UserName { get; set; } = "guest";

        /// <summary>
        /// Password of the chosen user.
        /// </summary>
        public string Password { get; set; } = "guest";

        /// <summary>
        /// Virtual host.
        /// </summary>
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// Automatic connection recovery option.
        /// </summary>
        public bool AutomaticRecoveryEnabled { get; set; } = true;

        /// <summary>
        /// Topology recovery option.
        /// </summary>
        public bool TopologyRecoveryEnabled { get; set; } = true;

        /// <summary>
        /// Timeout for connection attempts.
        /// </summary>
        public int RequestedConnectionTimeout { get; set; } = 60000;

        /// <summary>
        /// Heartbeat timeout.
        /// </summary>
        public ushort RequestedHeartbeat { get; set; } = 60;

        /// <summary>
        /// Application-specific connection name, will be displayed in the management UI if RabbitMQ server supports it.
        /// </summary>
        public string ClientProvidedName { get; set; }
    }
}