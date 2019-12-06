namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// Model of AMPQ Tcp endpoint.
    /// </summary>
    public class RabbitMqTcpEndpoint
    {
        /// <summary>
        /// RabbitMQ server.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Tcp connection port.
        /// </summary>
        public int Port { get; set; }
    }
}