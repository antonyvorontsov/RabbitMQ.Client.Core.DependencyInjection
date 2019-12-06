using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// RabbitMQ configuration model.
    /// </summary>
    public class RabbitMqClientOptions
    {
        /// <summary>
        /// Collection of AMPQ TCP endpoints.
        /// It can be used when RabbitMQ HA cluster is set up and you have to connect multiple hosts with different ports.
        /// </summary>
        /// <remarks>
        /// Has the first priority between properties TcpEndpoints, HostNames and HostName.
        /// If all properties are set, TcpEndpoints will be used.
        /// </remarks>
        public IEnumerable<RabbitMqTcpEndpoint> TcpEndpoints { get; set; }

        /// <summary>
        /// Collection of RabbitMQ server host names.
        /// It can be used when RabbitMQ HA cluster is set up and you have to connect multiple hosts.
        /// If HostNames collection is null or empty then HostName will be used to create connection.
        /// Otherwise HostNames collection will be used and HostName property value will be ignored.
        /// </summary>
        /// <remarks>
        /// Has the second priority between properties TcpEndpoints, HostNames and HostName.
        /// If HostNames collection property and HostName property both set then HostNames will be used.
        /// </remarks>
        public IEnumerable<string> HostNames { get; set; }

        /// <summary>
        /// RabbitMQ server.
        /// </summary>
        /// <remarks>
        /// Has the third priority between properties TcpEndpoints, HostNames and HostName.
        /// HostName will be used if only TcpEndpoints and HostNames properties are not set.
        /// </remarks>
        public string HostName { get; set; } = "127.0.0.1";

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